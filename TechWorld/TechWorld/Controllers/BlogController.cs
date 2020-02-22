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
    public class BlogController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd = null;
        readonly Connectionstring cs = new Connectionstring();
        readonly List<BlogModel> LBM = new List<BlogModel>();

        // GET: Blog
        public ActionResult Index(string RowsShow, string Page, string CategoryID)
        {
            var custModel = GetBlog();
            ViewBag.TotalPages = Math.Ceiling(custModel.Count() / 9.9);
            int page = int.Parse(Page == null ? "1" : Page);
            ViewBag.Page = page;
            RowsShow = (RowsShow == null || RowsShow == "" ? "9" : RowsShow);
            custModel = custModel.Skip((page - 1) * 9).Take(Convert.ToInt32(RowsShow)).ToList();
            CM.blogModel = custModel;
            return View(CM);
        }

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
                                    Month = rdr["Month"].ToString(),
                                    Day = rdr["Day"].ToString(),
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
    }
}