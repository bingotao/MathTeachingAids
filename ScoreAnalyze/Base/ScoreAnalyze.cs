using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreAnalyze
{
    class ScoreAnalyze
    {

        public static void Analyze()
        {
            var students = ScoreAnalyze.GetStudents(@"E:\others\04 MathTeachingAids\ScoreAnalyze\Documents\七年级登分表3.20.xlsx", "七年级成绩（可排序)");

            var totalCount = students.Count;
            var classNames = (from s in students select s.ClassName).Distinct().OrderBy(s => s).ToList();
            var classes = new List<Class>();
            ScoreAnalyze.RankAllSubjects(students);

            //  全体学生分数及各科排名
            var sb = new StringBuilder("姓名\t班级\t语文\t\t数学\t\t英语\t\t科学\t\t社会\t\t总分\t\n");
            foreach (var s in students)
            {
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\n",
                    s.Name, s.ClassName,
                    s.ChineseScore, s.ChineseRank,
                    s.MathScore, s.MathRank,
                    s.EnglishScore, s.EnglishRank,
                    s.ScienceScore, s.ScienceRank,
                    s.SocietyScore, s.SocietyRank,
                    s.Score, s.Rank);
            }
            var studentsScore = sb.ToString();

            //  班级成绩分析

            sb = new StringBuilder("班级\t人数\t平均分\t前10\t前20\t前50\t前80\t后20\n");
            foreach (var c in classNames)
            {
                var sTemp = students.Where(s => s.ClassName == c).ToList();
                var cls = new Class(c, sTemp, totalCount);
                classes.Add(cls);
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n",
                    c, cls.StudentCount, cls.AverageScore,
                    cls.Top10Count, cls.Top20Count, cls.Top50Count, cls.Top80Count, cls.Last20Count);
            }
            classes.Add(new Class("7年级", students, totalCount));
            var classAnalyze = sb.ToString();

            //  班级学科成绩分析

            sb = new StringBuilder("学科\t班级\t分数>=95\t95>分数>=90\t90>分数>=85\t85>分数>=80\t80>分数>=75\t75>分数>=70\t70>分数>=60\t分数<60\tA分\tA数\tA率\tE分\tE数\tE率\n");
            double[,] ranges = new double[,] {
                { 95,double.MaxValue},
                { 90,95},
                { 85,90},
                { 80,85},
                { 75,80},
                { 70,75},
                { 60,70},
                { double.MinValue,60}
            };
            //string[] subjects = new string[] { "Chinese", "Math", "English", "Society", "Science" };

            foreach (var subject in Subject.Subjects)
            {
                foreach (var c in classes)
                {
                    var subjectName = subject.Name;

                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\n",
                        subject.Aliases[0], c.ClassName,
                        c.GetScoreRangeCount(subjectName, ranges[0, 0], ranges[0, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[1, 0], ranges[1, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[2, 0], ranges[2, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[3, 0], ranges[3, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[4, 0], ranges[4, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[5, 0], ranges[5, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[6, 0], ranges[6, 1], subject.TotalScore),
                        c.GetScoreRangeCount(subjectName, ranges[7, 0], ranges[7, 1], subject.TotalScore),

                        c.GetAScore(subjectName, totalCount),
                        c.GetACount(subjectName, totalCount),
                        c.GetARatio(subjectName, totalCount),
                        c.GetEScore(subjectName, totalCount),
                        c.GetECount(subjectName, totalCount),
                        c.GetERatio(subjectName, totalCount)
                        );
                }
            }

            string classSubjectAnalyze = sb.ToString();
        }


        public static List<Student> GetStudents(string filePath, string workSheetName)
        {
            DataTable dtExcel = new DataTable();
            List<Student> students = new List<Student>();
            using (OleDbConnection con =
                new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended properties=\"Excel 12.0;Imex=1;HDR=Yes;\"", filePath)))
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("Select * from [" + workSheetName + "$]", con);
                con.Open();
                adapter.FillSchema(dtExcel, SchemaType.Mapped);
                adapter.Fill(dtExcel);
                con.Close();

                string nameField = null,
                        classField = null,
                        chineseField = null,
                        mathField = null,
                        englishField = null,
                        societyField = null,
                        scienceField = null;

                foreach (var alias in Header.Headers.Where(p => p.Name == "Name").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        nameField = alias;
                        break;
                    }
                }

                foreach (var alias in Header.Headers.Where(p => p.Name == "Class").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        classField = alias;
                        break;
                    }
                }

                foreach (var alias in Subject.Subjects.Where(p => p.Name == "Chinese").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        chineseField = alias;
                        break;
                    }
                }

                foreach (var alias in Subject.Subjects.Where(p => p.Name == "Math").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        mathField = alias;
                        break;
                    }
                }

                foreach (var alias in Subject.Subjects.Where(p => p.Name == "English").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        englishField = alias;
                        break;
                    }
                }

                foreach (var alias in Subject.Subjects.Where(p => p.Name == "Society").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        societyField = alias;
                        break;
                    }
                }

                foreach (var alias in Subject.Subjects.Where(p => p.Name == "Science").FirstOrDefault().Aliases)
                {
                    if (dtExcel.Columns.Contains(alias))
                    {
                        scienceField = alias;
                        break;
                    }
                }


                foreach (DataRow dr in dtExcel.Rows)
                {
                    students.Add(new Student()
                    {
                        Name = dr[nameField].ToString(),
                        ClassName = dr[classField].ToString(),
                        ChineseScore = (double)dr[chineseField],
                        MathScore = (double)dr[mathField],
                        EnglishScore = (double)dr[englishField],
                        ScienceScore = (double)dr[scienceField],
                        SocietyScore = (double)dr[societyField]
                    });
                }
            }

            return students;
        }

        public static void RankStudents(List<Student> students, string subject)
        {
            students = students.OrderByDescending(s => s[subject].Score).ToList();

            for (int i = 0, l = students.Count; i < l; i++)
            {
                var rank = i + 1;
                var score = students[i][subject].Score;
                if (i - 1 >= 0)
                {
                    var lastScore = students[i - 1][subject].Score;

                    if (lastScore == score)
                    {
                        rank = students[i - 1][subject].Rank;
                    }
                }
                students[i][subject] = new SubjectScore
                {
                    Name = subject,
                    Score = score,
                    Rank = rank
                };
            }
        }

        public static void RankAllSubjects(List<Student> students)
        {
            foreach (var subject in Subject.Subjects)
            {
                RankStudents(students, subject.Name);
            }
            RankStudents(students, "");
        }
    }
}
