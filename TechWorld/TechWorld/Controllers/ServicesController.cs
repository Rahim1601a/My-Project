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
    public class ServicesController : Controller
    {
        CustomModel CM = new CustomModel();
        SqlCommand cmd = null;
        readonly Connectionstring cs = new Connectionstring();
        readonly List<ServicesModel> LSM = new List<ServicesModel>();
        
        public ActionResult Index(string RowsShow, string Page, string CategoryID)
        {
            var custModel = GetServices();
            ViewBag.TotalPages = Math.Ceiling(custModel.Count() / 9.9);
            int page = int.Parse(Page == null ? "1" : Page);
            ViewBag.Page = page;
            RowsShow = (RowsShow == null || RowsShow == "" ? "9" : RowsShow);
            custModel = custModel.Skip((page - 1) * 9).Take(Convert.ToInt32(RowsShow)).ToList();
            CM.ServicesModel = custModel;
            return View(CM);
        }

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
    }
}