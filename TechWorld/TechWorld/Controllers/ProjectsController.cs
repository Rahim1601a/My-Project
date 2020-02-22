using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechWorld.Models;

namespace TechWorld.Controllers
{
    public class ProjectsController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd = null;
        readonly Connectionstring cs = new Connectionstring();
        readonly List<ProjectModel> LPM = new List<ProjectModel>();
        readonly List<ProjectCategoryModel> PCM = new List<ProjectCategoryModel>();

        // GET: Projects
        public ActionResult Index(string RowsShow, string Page,string CategoryID)
        {
            if (CategoryID != null)
            {
                ViewBag.category = CategoryID;
                var custModel = GetProject(CategoryID);
                ViewBag.TotalPages = Math.Ceiling(custModel.Count() / 9.9);
                int page = int.Parse(Page == null ? "1" : Page);
                ViewBag.Page = page;
                RowsShow = (RowsShow == null || RowsShow == "" ? "9" : RowsShow);
                custModel = custModel.Skip((page - 1) * 9).Take(Convert.ToInt32(RowsShow)).ToList();
                CM.projectModel = custModel;
                CM.projectCategoryModel = GetCategory();
                return View(CM);
            }
            else
            {
                var custModel = GetProject();
                ViewBag.TotalPages = Math.Ceiling(custModel.Count() / 9.9);
                int page = int.Parse(Page == null ? "1" : Page);
                ViewBag.Page = page;
                RowsShow = (RowsShow == null || RowsShow == "" ? "9" : RowsShow);
                custModel = custModel.Skip((page - 1) * 9).Take(Convert.ToInt32(RowsShow)).ToList();
                CM.projectModel = custModel;
                CM.projectCategoryModel = GetCategory();
                return View(CM);
            }
        }
        
        public ActionResult ProjectView()
        {
            //CM.projectModel = GetProject(CategoryID);

            //return View(CM);
            return View();
        }

        [HttpGet]
        public List<ProjectModel> GetProject(string Category)
        {
            LPM.Clear();
            if (Category == null || Category == "1")
            {
                Category = "";
            }
            string sp = "Sp_GetProject";
            using (SqlConnection con = new SqlConnection(cs.Constr))
            {
                con.Open();
                cmd = con.CreateCommand();
                SqlTransaction transaction;

                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd = new SqlCommand(sp, con)
                {
                    Transaction = transaction
                };

                try
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectType", Category.ToString());
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ProjectModel PM = new ProjectModel
                                {
                                    ID = (int)(rdr["ID"]),
                                    Tittle = rdr["Project_TItle"].ToString(),
                                    ProjectTypeName = rdr["CategoryName"].ToString(),
                                    Description = rdr["Project_Description"].ToString(),
                                    ProjectYear = rdr["Project_Year"].ToString(),
                                    ProjectHead = rdr["Project_Head"].ToString(),
                                    ProjectType = (int)rdr["Project_CategoryID"],
                                    Image = rdr["Project_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LPM.Add(PM);
                            }
                        }
                    }

                    transaction.Commit();
                    con.Close();
                }
                catch (Exception e)
                {
                    string result;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        if (transaction.Connection != null)
                        {
                            result = ex.Message;
                        }
                    }
                    result = e.Message;
                }
            }
            return LPM;
        }

        [HttpGet]
        public List<ProjectModel> GetProject()
        {
            string Category = "";
            LPM.Clear();
            if (Category == null || Category == "1")
            {
                Category = "";
            }
            string sp = "Sp_GetProject";
            using (SqlConnection con = new SqlConnection(cs.Constr))
            {
                con.Open();
                cmd = con.CreateCommand();
                SqlTransaction transaction;

                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd = new SqlCommand(sp, con)
                {
                    Transaction = transaction
                };

                try
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectType", Category.ToString());
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ProjectModel PM = new ProjectModel
                                {
                                    ID = (int)(rdr["ID"]),
                                    Tittle = rdr["Project_TItle"].ToString(),
                                    ProjectTypeName = rdr["CategoryName"].ToString(),
                                    Description = rdr["Project_Description"].ToString(),
                                    ProjectYear = rdr["Project_Year"].ToString(),
                                    ProjectHead = rdr["Project_Head"].ToString(),
                                    ProjectType = (int)rdr["Project_CategoryID"],
                                    Image = rdr["Project_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LPM.Add(PM);
                            }
                        }
                    }

                    transaction.Commit();
                    con.Close();
                }
                catch (Exception e)
                {
                    string result;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        if (transaction.Connection != null)
                        {
                            result = ex.Message;
                        }
                    }
                    result = e.Message;
                }
            }
            return LPM;
        }

        [HttpGet]
        public List<ProjectCategoryModel> GetCategory()
        {
            string sp = "Sp_GetCategory";
            using (SqlConnection con = new SqlConnection(cs.Constr))
            {
                con.Open();
                cmd = con.CreateCommand();
                SqlTransaction transaction;

                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd = new SqlCommand(sp, con)
                {
                    Transaction = transaction
                };

                try
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ProjectCategoryModel PM = new ProjectCategoryModel
                                {
                                    ID = (int)(rdr["ID"]),
                                    CategoryName = rdr["CategoryName"].ToString(),
                                };
                                PCM.Add(PM);
                            }
                        }
                    }

                    transaction.Commit();
                    con.Close();
                }
                catch (Exception e)
                {
                    string result;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        if (transaction.Connection != null)
                        {
                            result = ex.Message;
                        }
                    }
                    result = e.Message;
                }
            }
            return PCM;
        }

        [HttpGet]
        public List<ProjectModel> GetProject(int ID)
        {
            string sp = "Sp_GetProjectWithID";
            using (SqlConnection con = new SqlConnection(cs.Constr))
            {
                con.Open();
                cmd = con.CreateCommand();
                SqlTransaction transaction;

                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd = new SqlCommand(sp, con)
                {
                    Transaction = transaction
                };

                try
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectID", ID);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ProjectModel PM = new ProjectModel
                                {
                                    ID = (int)(rdr["ID"]),
                                    Tittle = rdr["Project_TItle"].ToString(),
                                    ProjectTypeName = rdr["CategoryName"].ToString(),
                                    Description = rdr["Project_Description"].ToString(),
                                    ProjectYear = rdr["Project_Year"].ToString(),
                                    ProjectHead = rdr["Project_Head"].ToString(),
                                    Image = rdr["Project_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LPM.Add(PM);
                            }
                        }
                    }

                    transaction.Commit();
                    con.Close();
                }
                catch (Exception e)
                {
                    string result;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        if (transaction.Connection != null)
                        {
                            result = ex.Message;
                        }
                    }
                    result = e.Message;
                }
            }
            return LPM;
        }
    }
}