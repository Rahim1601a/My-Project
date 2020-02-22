using AdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminPanel.Controllers
{
    public class ProjectsController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd, cmd1 = null;
        readonly Connectionstring cs = new Connectionstring();       
        readonly List<ProjectModel> LPM = new List<ProjectModel>();

        [HttpGet]
        public List<ProjectModel> GetProject()
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Insert(ProjectModel Model, HttpPostedFileBase[] file)
        {
            var res1 = 0;
            string sp = "Sp_InsertProject";
            string sp1 = "Sp_InsertProjectmage";
            string result = "Error!";
            try
            {
                using (SqlConnection con = new SqlConnection(cs.Constr))
                {
                    con.Open();
                    cmd = con.CreateCommand();
                    cmd1 = con.CreateCommand();
                    SqlTransaction transaction;

                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                    cmd = new SqlCommand(sp, con);
                    cmd1 = new SqlCommand(sp1, con);

                    cmd.Transaction = transaction;
                    cmd1.Transaction = transaction;

                    try
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Project_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Project_Description", Model.Description);
                        cmd.Parameters.AddWithValue("@Project_Type", Model.ProjectType);
                        cmd.Parameters.AddWithValue("@Project_Year", Model.ProjectYear);
                        cmd.Parameters.AddWithValue("@Project_Head", Model.ProjectHead);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        int PK_ID = (int)(cmd.ExecuteScalar());
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Projects");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                string path = Path.Combine(folderPath, Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Project_Image", Combine);
                                cmd1.Parameters.AddWithValue("@ProjectID", PK_ID);
                                cmd1.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                                cmd1.Parameters.AddWithValue("@IsActive", Model.IsActive);
                                res1 = cmd1.ExecuteNonQuery();
                                cmd1.Parameters.Clear();
                                
                            }
                        }
                        if (res1 > 0)
                        {
                            result = "Added Successfully!";
                        }
                        cmd.Parameters.Clear();
                        transaction.Commit();
                        con.Close();
                    }
                    catch (Exception e)
                    {
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
            }

            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(ProjectModel Model, HttpPostedFileBase[] file)
        {
            var res1 = 0;
            string sp = "Sp_UpdateProject";
            string sp1 = "Sp_UpdateProjectmage";
            string result = "Error!";
            try
            {
                using (SqlConnection con = new SqlConnection(cs.Constr))
                {
                    con.Open();
                    cmd = con.CreateCommand();
                    cmd1 = con.CreateCommand();
                    SqlTransaction transaction;

                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                    cmd = new SqlCommand(sp, con);
                    cmd1 = new SqlCommand(sp1, con);

                    cmd.Transaction = transaction;
                    cmd1.Transaction = transaction;

                    try
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Project_ID ", Model.ID);
                        cmd.Parameters.AddWithValue("@Project_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Project_Description", Model.Description);
                        cmd.Parameters.AddWithValue("@Project_Type", Model.ProjectType);
                        cmd.Parameters.AddWithValue("@Project_Year", Model.ProjectYear);
                        cmd.Parameters.AddWithValue("@Project_Head", Model.ProjectHead);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Projects");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                string path = Path.Combine(folderPath, Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Project_Image", Combine);
                                cmd1.Parameters.AddWithValue("@ID", Model.ID);
                                cmd1.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                                cmd1.Parameters.AddWithValue("@IsActive", Model.IsActive);
                                res1 = cmd1.ExecuteNonQuery();
                                cmd1.Parameters.Clear();

                            }
                        }
                        if (res1 > 0)
                        {
                            result = "Update Successfully!";
                        }
                        cmd.Parameters.Clear();
                        transaction.Commit();
                        con.Close();
                    }
                    catch (Exception e)
                    {
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
            }

            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int ID)
        {
            string sp = "Sp_DeleteProject";
            string sp1 = "Sp_DeleteProjectImage";
            string result;
            try
            {
                result = "Error!";

                using (SqlConnection con = new SqlConnection(cs.Constr))
                {
                    con.Open();
                    cmd = con.CreateCommand();
                    cmd1 = con.CreateCommand();
                    SqlTransaction transaction;

                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                    cmd = new SqlCommand(sp, con);
                    cmd1 = new SqlCommand(sp1, con);
                    cmd.Transaction = transaction;
                    cmd1.Transaction = transaction;
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Project_ID ", ID);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Project_ID", ID);
                        int res1 = cmd1.ExecuteNonQuery();
                        cmd1.Parameters.Clear();

                        if (res1 > 0)
                        {
                            result = "Delete Successfully!";
                        }
                        cmd.Parameters.Clear();
                        transaction.Commit();
                        con.Close();
                    }
                    catch (Exception e)
                    {
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
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            CM.projectModel = GetProject();
            return View(CM);
        }
    }
}