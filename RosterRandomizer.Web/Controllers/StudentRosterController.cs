using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RosterRandomizer.Web.Models;

namespace RosterRandomizer.Web.Controllers {
    public class StudentRosterController : Controller {
        //https://www.mikesdotnetting.com/Article/302/server-mappath-equivalent-in-asp-net-core
        IWebHostEnvironment _HostingEnvironment;
        private static readonly Dictionary<string, SemaphoreSlim> _SlimDict = new Dictionary<string, SemaphoreSlim>();
        public StudentRosterController(IWebHostEnvironment ihost) {
            _HostingEnvironment = ihost;
        }


        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// https://code-maze.com/file-upload-aspnetcore-mvc/
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FromFile(IFormFile file) {

            long size = file.Length;

            if (size > 0) {
                // has content
                string newCode = Hasher.getRandomKey();
                string newFilePath = getFilePath(newCode);
                populateSemaphore(newCode);
                await _SlimDict[newCode].WaitAsync();
                try {
                    using (FileStream str = new FileStream(newFilePath, FileMode.Create)) {
                        await file.CopyToAsync(str);
                    }
                } finally {
                    _SlimDict[newCode].Release();
                }
                return RedirectToAction("FromCode", new { code = newCode });
            }
            return RedirectToAction("Index", "Home");
        }

        private string getFilePath(string fileName) {
            // make sure name is all caps.
            fileName = fileName.ToUpper();
            string dataFolder = (_HostingEnvironment.ContentRootPath + @"\Data");
            string newFilePath = dataFolder + @"\" + fileName + ".json";
            return newFilePath;
        }

        public async Task<IActionResult> FromCode(string code, string enteredcode) {

            if (!string.IsNullOrEmpty(enteredcode)) {
                code = enteredcode;
            }

            List<Student> students = await GetStudentFromCode(code);
            //TODO: Need to make sure file exists.

            // check to see if upgraded file type
            int numberNotExported = students.Count(s => !s.IsFromExported);
            // assume if not Exported then it is a fresh list and needs to be converted to new format.
            if (numberNotExported > 0) {
                // If not, convert to new upgraded JSON list
            }

            ViewBag.code = code;
            return View(students);
        }

        private async Task<List<Student>> GetStudentFromCode(string code) {
            List<Student> retList;
            string filePath = getFilePath(code);
            string fileContent;
            populateSemaphore(code);
            await _SlimDict[code].WaitAsync();
            try {
                fileContent = await System.IO.File.ReadAllTextAsync(filePath);
            } finally {
                _SlimDict[code].Release();
            }
            retList = DataStore.ParseStudents(fileContent);
            return retList;
        }

        private void populateSemaphore(string code) {
            // make sure code has a semaphore
            if (!_SlimDict.ContainsKey(code)) {
                _SlimDict.Add(code, new SemaphoreSlim(1, 1));
            }

        }

        private async Task<int> UpdateStudents(List<Student> students, string code) {
            string newJson = DataStore.ConvertToJSON(students);
            string filePath = getFilePath(code);

            populateSemaphore(code);
            await _SlimDict[code].WaitAsync();
            try {
                await System.IO.File.WriteAllTextAsync(filePath, newJson);
            } finally {
                _SlimDict[code].Release();
            }
            return 1;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(string code, int studentid, bool isSelected, bool inClass, bool reset) {
            List<Student> students = await GetStudentFromCode(code);
            Student stud = students.FirstOrDefault(s => s.ID == studentid);

            if (stud != null) {
                if (reset) {
                    stud.IsSelected = false;
                    stud.InClass = true;
                } else {
                    stud.IsSelected = isSelected;
                    stud.InClass = inClass;
                }
            }
            int answer = await UpdateStudents(students, code);
            return PartialView("Parts/Card", stud);
        }

        [HttpPost]
        public async Task<IActionResult> GetRandom(string code) {
            List<Student> students = await GetStudentFromCode(code);
            int countAvailable = students.Count(s => s.InClass == true
                                                 && s.IsSelected == false);
            ViewBag.idHelper = "random";
            if (countAvailable == 0) {
                // reset list.
                foreach (Student s in students) {
                    s.IsSelected = false;
                }
                // indicate that the whole list has been reset and the GUI needs refreshed.
                ViewBag.idHelper = "reset";
            }
            Student stud = null;
            Random rnd = new Random();
            while (stud == null) {
                int nextID = rnd.Next(students.Count);
                // get student if not picked already and in class.
                stud = students.FirstOrDefault(s => s.ID == nextID
                                                 && s.InClass == true
                                                 && s.IsSelected == false);
            }
            // now mark student selected.
            //stud.IsSelected = true;
            // save list.
            int answer = await UpdateStudents(students, code);

            return PartialView("Parts/Card", stud);
        }


    }
}