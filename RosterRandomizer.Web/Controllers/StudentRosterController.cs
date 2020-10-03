using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RosterRandomizer.Web.Models;

namespace RosterRandomizer.Web.Controllers
{
    public class StudentRosterController : Controller
    {
        //https://www.mikesdotnetting.com/Article/302/server-mappath-equivalent-in-asp-net-core
        IHostingEnvironment _HostingEnvironment;
        public StudentRosterController(IHostingEnvironment ihost) {
            _HostingEnvironment = ihost;
        }


        public IActionResult Index()
        {
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

                using (FileStream str = new FileStream(newFilePath, FileMode.Create)) {
                   await file.CopyToAsync(str);
                }
                return RedirectToAction("FromCode", new { code = newCode });
            }
            return RedirectToAction("Index","Home");
        }

        private string getFilePath(string fileName) {
            // make sure name is all caps.
            fileName = fileName.ToUpper();
            string dataFolder = (_HostingEnvironment.ContentRootPath + @"\Data");
            string newFilePath = dataFolder + @"\" + fileName + ".json";
            return newFilePath;
        }

        public IActionResult FromCode(string code,string enteredcode) {
            
            if (!string.IsNullOrEmpty(enteredcode)) {
                code = enteredcode;
            }

            string filePath = getFilePath(code);
            List<Student> students;
            //TODO: Need to make sure file exists.

            string fileContent = System.IO.File.ReadAllText(filePath);
            students = DataStore.ParseStudents(fileContent);

            // check to see if upgraded file type
            int numberNotExported = students.Count(s => !s.IsFromExported);
            // assume if not Exported then it is a fresh list and needs to be converted to new format.
            if (numberNotExported > 0) {
                // If not, convert to new upgraded JSON list
                string newJson = DataStore.ConvertToJSON(students);
                System.IO.File.WriteAllText(filePath, newJson);
            }
            return View(students);
        }


        }
    }