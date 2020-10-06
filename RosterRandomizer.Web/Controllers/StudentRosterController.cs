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
        public StudentRosterController(IWebHostEnvironment ihost) {
            _HostingEnvironment = ihost;
        }


        public async Task<IActionResult> Index() {
            if (Request.Cookies["code"] != null) {
                string code = Request.Cookies["code"];
                List<Student> students = await GetStudents(code);
                //TODO: Need to make sure file exists.

                // check to see if upgraded file type
                int numberNotExported = students.Count(s => !s.IsFromExported);
                // assume if not Exported then it is a fresh list and needs to be converted to new format.
                if (numberNotExported > 0) {
                    // If not, convert to new upgraded JSON list
                    int num = await UpdateStudents(students, code);
                }
                ViewBag.code = code;
                return View(students);
            }

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
                await DataStore.Semaphores[newCode].WaitAsync();
                try {
                    using (FileStream str = new FileStream(newFilePath, FileMode.Create)) {
                        await file.CopyToAsync(str);
                    }
                } finally {
                    DataStore.Semaphores[newCode].Release();
                }
                List<Student> students = await GetStudents(newCode);
                // give each student a unique ID.
                int num = 0;
                foreach(Student st in students) {
                    st.ID = num++;
                }
                await UpdateStudents(students, newCode);
                return RedirectToAction("FromCode", new { code = newCode });
            } else {
                // File was empty.
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

        public IActionResult FromCode(string code) {
            if (!string.IsNullOrEmpty(code)) {
                Response.Cookies.Append("code", code);
                return RedirectToAction("Index");
            } else {
               return RedirectToAction("Index", "Home");
            }
        }

        private async Task<List<Student>> GetStudents(string code) {
            List<Student> retList;
            string filePath = getFilePath(code);
            string fileContent;
            populateSemaphore(code);
            await DataStore.Semaphores[code].WaitAsync();
            try {
                fileContent = await System.IO.File.ReadAllTextAsync(filePath);
            } finally {
                DataStore.Semaphores[code].Release();
            }
            retList = DataStore.ParseStudents(fileContent);
            return retList;
        }

        public async Task<IActionResult> GetStudentsFile(string code) {
            List<Student> students = await GetStudents(code);
            try {
                using (FileStream fs = new FileStream(getFilePath(code), FileMode.Open)) {

                    return File(fs, "text/json", code + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".json");
                }
            } catch {
                return NotFound();
            }
        }

        private void populateSemaphore(string code) {
            // make sure code has a semaphore
            if (!DataStore.Semaphores.ContainsKey(code)) {
                DataStore.Semaphores.Add(code, new SemaphoreSlim(1, 1));
            }

        }

        private async Task<int> UpdateStudents(List<Student> students, string code) {
            string newJson = DataStore.ConvertToJSON(students);
            string filePath = getFilePath(code);

            populateSemaphore(code);
            await DataStore.Semaphores[code].WaitAsync();
            try {
                await System.IO.File.WriteAllTextAsync(filePath, newJson);
            } finally {
                DataStore.Semaphores[code].Release();
            }
            return 1;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(string code, int studentid, bool isSelected, bool inClass, bool reset) {
            List<Student> students = await GetStudents(code);
            Student stud = students.FirstOrDefault(s => s.ID == studentid);

            if (stud != null) {
                if (reset) {
                    stud.IsSelected = false;
                    stud.InClass = true;
                } else {
                    stud.IsSelected = isSelected && inClass;
                    stud.InClass = inClass;
                }
            }
            int answer = await UpdateStudents(students, code);
            return PartialView("Parts/Card", stud);
        }

        [HttpPost]
        public async Task<IActionResult> GetRandom(string code) {
            List<Student> students = await GetStudents(code);
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
            
            return PartialView("Parts/Card", stud);
        }

        [HttpPost]
        public async Task<IActionResult> GetList(string code) {
            List<Student> students = await GetStudents(code);
            return PartialView("Parts/List", students);
        }

    }
}