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
    public class BlogController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd, cmd1 = null;
        readonly Connectionstring cs = new Connectionstring();        
        readonly List<BlogModel> LBM = new List<BlogModel>();
        
        [HttpGet]
        public List<BlogModel> GetBlog()
        {
            string sp = "Sp_GetBlog";
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
                                BlogModel BM = new BlogModel
                                {
                                    ID = Convert.ToInt32(rdr["ID"]),
                                    Tittle = rdr["Blog_Tittle"].ToString(),
                                    BlogContent = rdr["Blog_Content"].ToString(),
                                    CreateBy = rdr["CreateBy"].ToString(),
                                    Image = rdr["Blog_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LBM.Add(BM);
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
            return LBM;
        }

        [HttpGet]
        public List<BlogModel> GetBlog(int ID)
        {
            string sp = "Sp_GetBlogWithID";
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
                    cmd.Parameters.AddWithValue("@BlogID", ID);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                BlogModel BM = new BlogModel
                                {
                                    ID = Convert.ToInt32(rdr["ID"]),
                                    Tittle = rdr["Blog_Tittle"].ToString(),
                                    BlogContent = rdr["Blog_Content"].ToString(),
                                    CreateBy = rdr["CreateBy"].ToString(),
                                    Image = rdr["Blog_Image"].ToString(),
                                    IsActive = rdr["IsActive"].ToString()
                                };
                                LBM.Add(BM);
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
            return LBM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Insert(BlogModel Model, HttpPostedFileBase[] file)
        {
            string sp = "Sp_InsertBlogs";
            string sp1 = "Sp_InsertBlogImage";
            var res1 = 0;
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
                        cmd.Parameters.AddWithValue("@Blog_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Create_By", Model.CreateBy);
                        cmd.Parameters.AddWithValue("@Blog_Content", Model.BlogContent);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        int PK_ID = Convert.ToInt32(cmd.ExecuteScalar());
                        //cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Blogs");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                //string MainPath = "~/Images/";
                                string path = Path.Combine(folderPath, Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Blog_Image", Combine);
                                cmd1.Parameters.AddWithValue("@Blog_ID", PK_ID);
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
        public JsonResult Update(BlogModel Model, HttpPostedFileBase[] file)
        {
            string sp = "Sp_UpdateBlogs";
            string sp1 = "Sp_UpdateBlogImage";
            var res1 = 0;
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
                        cmd.Parameters.AddWithValue("@Blog_ID ", Model.ID);
                        cmd.Parameters.AddWithValue("@Blog_Tittle ", Model.Tittle);
                        cmd.Parameters.AddWithValue("@Create_By", Model.CreateBy);
                        cmd.Parameters.AddWithValue("@Blog_Content", Model.BlogContent);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", Model.IsActive);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string folderPath = Server.MapPath("~/Images/Blogs");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFileBase files in file)
                        {
                            //Checking file is available to save.  
                            if (files != null)
                            {
                                //string MainPath = "~/Images/";
                                string path = Path.Combine(folderPath, Path.GetFileName(files.FileName));
                                files.SaveAs(path);
                                string Combine = folderPath + files.FileName;
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@Blog_Image", Combine);
                                cmd1.Parameters.AddWithValue("@ID", Model.ID);
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
        public JsonResult Delete(int ID)
        {
            string sp = "Sp_DeleteBlogs";
            string sp1 = "Sp_DeleteBlogImage";
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
                        cmd.Parameters.AddWithValue("@Blog_ID ", ID);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Blog_ID", ID);
                        int res1 = cmd1.ExecuteNonQuery();
                        cmd1.Parameters.Clear();

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

        public ActionResult Index()
        {
            CM.blogModel = GetBlog();
            return View(CM);            
        }
    }
}