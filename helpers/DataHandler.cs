using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CivSem1Challenge2_RegistrationSystem.models;
using CsvHelper;
using CsvHelper.Configuration;

namespace CivSem1Challenge2_RegistrationSystem.helpers {
    public class DataHandler {


        public List<Course> GetCourses () {
            var config = new CsvConfiguration (CultureInfo.InvariantCulture) {
                PrepareHeaderForMatch = args => args.Header.ToLower (),
            };

            IEnumerable<Data> data;
            List<Data> dataList;
            using (var reader = new StreamReader ("student_data_mock.csv"))
            using (var csv = new CsvReader (reader, config)) {
                data = csv.GetRecords<Data> ();
                dataList = data.ToList<Data> ();
            }

            return this.GetCoursesHelper (dataList);
        }

        public List<Course> GetCoursesHelper (List<Data> data) {

            var config = new CsvConfiguration (CultureInfo.InvariantCulture) {
                PrepareHeaderForMatch = args => args.Header.ToLower (),
            };

            IEnumerable<Course> courseData;
            List<Course> courseList;
            using (var reader = new StreamReader ("course_data.csv"))
            using (var csv = new CsvReader (reader, config)) {
                courseData = csv.GetRecords<Course> ();
                courseList = courseData.ToList<Course> ();
            }

            for (int i = 0; i < data.Count; i++) {
                var d = data[i];
                var student = new Student (d.FirstName, d.Surname, d.YearOfBirth, d.MonthOfBirth, d.DateOfBirth, d.StudentNo, d.FirstRegistrationYear);
                for (int j = 0; j < courseList.Count; j++) {
                    Course c = courseList[j];
                    if (d.CourseNo == c.CourseNo) {
                        c.Enrolments.Add (student);
                        break;
                    }
                }
            }

            return courseList;

        }

        public List<Student> GetStudents () {

            var config = new CsvConfiguration (CultureInfo.InvariantCulture) {
                PrepareHeaderForMatch = args => args.Header.ToLower (),
            };

            IEnumerable<Student> data;
            List<Student> studentList;
            using (var reader = new StreamReader ("student_data_mock.csv"))
            using (var csv = new CsvReader (reader, config)) {
                data = csv.GetRecords<Student> ();
                studentList = data.ToList<Student> ();
            }

            studentList.Shuffle();

            return studentList; 
        }

        public void SaveStudents(List<Course> courses, List<Student> unenrolled){
            
            using (var file = File.CreateText("student_data_mock.csv"))
            {
                string[] line = new string[] {"firstName","surname","yearOfBirth","monthOfBirth","dateOfBirth","studentNo","FirstRegistrationYear","courseNo"};
                
                // write the header
                file.WriteLine(string.Join(",", line));
                
                // write the enrolled students
                foreach(var c in courses)
                {
                    foreach(var s in c.Enrolments)
                    {
                        line = new string[] {s.FirstName, s.Surname, s.YearOfBirth.ToString(), 
                                            s.MonthOfBirth.ToString(), s.DateOfBirth.ToString(), 
                                            s.StudentNo.ToString(), s.FirstRegistrationYear.ToString(),
                                            c.CourseNo.ToString()};

                        file.WriteLine(string.Join(",", line));
                    }
                }

                // write the unenrolled students
                foreach(var s in unenrolled){
                        line = new string[] {s.FirstName, s.Surname, s.YearOfBirth.ToString(), 
                                            s.MonthOfBirth.ToString(), s.DateOfBirth.ToString(), 
                                            s.StudentNo.ToString(), s.FirstRegistrationYear.ToString(),
                                            "0"};

                        file.WriteLine(string.Join(",", line));
                
                }
            } // end using
        }
    }
}