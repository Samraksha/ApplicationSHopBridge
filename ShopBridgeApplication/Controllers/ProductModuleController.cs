using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShopBridgeApplication.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;


namespace ShopBridgeApplication.Controllers
{
    public class ProductModuleController : ApiController
    {

   
        // GET api/<controller>/5
        public IHttpActionResult GetProductDetails(string ProductName="",string ProductCategory="",DateTime? dtFromDate=null,DateTime? dtToDate=null)
        {
            
            List<Product> prodDetails = new List<Product>();
         
            SqlDataReader rd;
          
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconnShopBrid"].ConnectionString.ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "GetProductDetails";
                        cmd.CommandType = CommandType.StoredProcedure;
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            Product prod = new Product();

                            prod.ProductId = Convert.ToInt32(rd["ProductID"].ToString());
                            prod.ProductName = Convert.ToString(rd["ProductName"].ToString());
                            prod.ProductCategory = Convert.ToString(rd["ProductCategory"].ToString());
                            prod.ProductPrice = Convert.ToDecimal(rd["ProductPrice"].ToString());
                            prod.ProductImagePath = Convert.ToString(rd["ProductImagePath"].ToString());
                            prod.ModifiedDateTime = Convert.ToDateTime(rd["ModifiedDateTime"].ToString());
                            prod.CreatedDateTime = Convert.ToDateTime(rd["CreatedDateTime"].ToString());
                            prodDetails.Add(prod);

                        }
                        rd.Close();
                    }
                    connection.Close();

                }


                var FilteredData = prodDetails.Where(p => p.ProductName.ToLower().Contains(ProductName.ToLower() == "" ? p.ProductName.ToLower() : ProductName.ToLower()))
                                                .Where(p => p.ProductCategory.ToLower().Contains(ProductCategory.ToLower() == "" ? p.ProductCategory.ToLower() : ProductCategory.ToLower())).Select(p => p);


                if (dtFromDate != null)
                    FilteredData = FilteredData.Where(p => p.CreatedDateTime > dtFromDate).Select(p => p);
                if (dtToDate != null)
                    FilteredData = FilteredData.Where(p => p.CreatedDateTime < dtToDate).Select(p => p);
                if (FilteredData.LongCount() == 0)
                    return NotFound();
                else
                    return Ok(FilteredData);
            }
            catch (Exception ex)
            {
                LoggingErrorDetails(ex.StackTrace.ToString() + Environment.NewLine + ex.Message.ToString(), "GetProductDetails", "ProductName : " + ProductName.ToString());
                return NotFound();
            }
           

        }

        // POST api/<controller>
        public HttpResponseMessage PostInsertProductDetails(Product productDetails)
        {
            object strStatus;
            var response = new HttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconnShopBrid"].ConnectionString.ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "InsertProductDetails";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = productDetails.ProductName;
                        cmd.Parameters.Add("@ProductCategory", SqlDbType.NVarChar).Value = productDetails.ProductCategory;
                        cmd.Parameters.Add("@ProductPrice", SqlDbType.Float).Value = productDetails.ProductPrice;
                        cmd.Parameters.Add("@ProductImagePath", SqlDbType.NVarChar).Value = productDetails.ProductImagePath;
                        strStatus = cmd.ExecuteScalar();


                    }
                    connection.Close();
                }
                if (strStatus == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NonAuthoritativeInformation);

                }
                else if (strStatus.ToString() == "Already Exists")
                { throw new HttpResponseException(HttpStatusCode.NotAcceptable); }
                else

                {

                    response.Headers.Add("InsertMessage", "Succsessfuly Inserted!");
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingErrorDetails(ex.StackTrace.ToString() + Environment.NewLine + ex.Message.ToString(), "PostInsertProductDetails", "productDetails : " + productDetails.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }

            //return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage PutProductDetails(int ProductId, Product productDetails)
        {
            object strStatus;
            var response = new HttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconnShopBrid"].ConnectionString.ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "UpdateProductDetails";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProductID", SqlDbType.NVarChar).Value = ProductId;
                        cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = productDetails.ProductName;
                        cmd.Parameters.Add("@ProductCategory", SqlDbType.NVarChar).Value = productDetails.ProductCategory;
                        cmd.Parameters.Add("@ProductPrice", SqlDbType.Float).Value = productDetails.ProductPrice;
                        cmd.Parameters.Add("@ProductImagePath", SqlDbType.NVarChar).Value = productDetails.ProductImagePath;
                        strStatus = cmd.ExecuteScalar();

                    }
                    connection.Close();
                }
                if (strStatus == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                }
                else

                {

                    response.Headers.Add("UpdateMessage", "Succsessfuly Updated!");
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingErrorDetails(ex.StackTrace.ToString() + Environment.NewLine + ex.Message.ToString(), "PutProductDetails", "ProductId : " + ProductId.ToString() + " productDetails"+ productDetails.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
          
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage DeleteProducts(int id)
        {
            object strStatus;
            var response = new HttpResponseMessage();
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconnShopBrid"].ConnectionString.ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "DeleteProduct";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = id;
                        strStatus = cmd.ExecuteScalar();

                    }
                    connection.Close();
                }
                if (strStatus == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                }
                else
                {

                    response.Headers.Add("DeleteMessage", "Succsessfuly Deleted!");
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingErrorDetails(ex.StackTrace.ToString() + Environment.NewLine + ex.Message.ToString(), "DeleteProducts", "id : " + id.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
           

        }


     
        public  void LoggingErrorDetails(string strExceptionDetails,String MethodName,string paramters)
        {
            var line = Environment.NewLine + Environment.NewLine;

           

            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory;// context.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path

                if (!Directory.Exists(filepath + "ExceptionDetailsFile/"))
                {
                    Directory.CreateDirectory(filepath+ "ExceptionDetailsFile/");

                }
                filepath = filepath + "ExceptionDetailsFile/" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {


                    File.Create(filepath);

                }
              
                 
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Method Name :" + MethodName + Environment.NewLine+ "Input Parameters to method :"+paramters+ Environment.NewLine + "Exception Details :" + strExceptionDetails;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }
    }
}