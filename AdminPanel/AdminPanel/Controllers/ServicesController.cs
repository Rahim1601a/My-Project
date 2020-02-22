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
    public class ServicesController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd, cmd1 = null;
        readonly Connectionstring cs = new Connectionstring(); 
        readonly List<ServicesModel> LSM = new List<ServicesModel>();        

        [HttpGet]
        public List<ServicesModel> GetServices()
        {
            string sp = "Sp_GetServices";
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
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ServicesModel SM = new ServicesModel
                                {
                                    ID = Convert.ToInt32(rdr["ID"]),
                                    Tittle = rdr["Services_Tittle"].ToString(),
                                    Description = rdr["Services_Description"].ToString(),
                                    Image = rdr["Services_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LSM.Add(SM);
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
            return LSM;
        }

        [HttpGet]
        public List<ServicesModel> GetServices(int ID)
        {
            string sp = "Sp_GetServicesWithID";
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
                    cmd.Parameters.AddWithValue("@ServicesID", ID);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ServicesModel SM = new ServicesModel
                                {
                                    ID = Convert.ToInt32(rdr["ID"]),
                                    Tittle = rdr["Services_Tittle"].ToString(),
                                    Description = rdr["Services_Description"].ToString(),
                                    Image = rdr["Services_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LSM.Add(SM);
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
            return LSM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Insert(ProjectModel Model, HttpPostedFileBase[] file)
        {
            var res1 = 0;
            string sp = "Sp_InsertServices";
            string sp1 = "Sp_InsertServicesImage";
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
                        cmd.Parameters.AddWithValue("@Services_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Services_Description", Model.Description);                        
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        int PK_ID = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Services");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                string path = Path.Combine(Server.MapPath(folderPath), Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Services_Image", Combine);
                                cmd1.Parameters.AddWithValue("@ServicesID", PK_ID);
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
            string sp = "Sp_UpdateServices";
            string sp1 = "Sp_UpdateServicesImage";
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
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Services_ID ", Model.ID);
                        cmd.Parameters.AddWithValue("@Services_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Services_Description", Model.Description);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Services");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                string path = Path.Combine(Server.MapPath(folderPath), Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Services_Image", Combine);
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
        public JsonResult Delete(int ID)
        {
            string sp = "Sp_DeleteServices";
            string sp1 = "Sp_DeleteServicesImage";
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
                        cmd.Parameters.AddWithValue("@Services_ID ", ID);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Services_ID", ID);
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
            CM.ServicesModel = GetServices();
            return View(CM);
        }
    }
}