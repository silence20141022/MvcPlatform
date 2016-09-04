﻿using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class OrderDomesticController : Controller
    {
        //
        // GET: /OrderDomestic/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        public string GetChk(string check_val)
        {
            return check_val == "on" ? "1" : "0";
        }

        public string GetName(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int index = combin.LastIndexOf("(");
                return combin.Substring(0, index);
            }
        }
        public string GetCode(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int start = combin.LastIndexOf("(");
                int end = combin.LastIndexOf(")");
                return combin.Substring(start + 1, end - start - 1);
            }
        }

        //删除订单
        public string Delete()
        {
            //删除订单
            string result = "";
            string sql = "select * from LIST_ORDER where code='" + Request["ordercode"] + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (!string.IsNullOrEmpty(dt.Rows[0]["CORRESPONDNO"] + ""))//如果四单
            {
                sql = "select * from LIST_ORDER where CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "'";
                dt = DBMgr.GetDataTable(sql);
                foreach (DataRow dr_t in dt.Rows)
                {
                    result = Extension.deleteorder(dr_t["CODE"] + "");
                }
            }
            else
            {
                sql = "select * from LIST_ORDER where ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                dt = DBMgr.GetDataTable(sql);
                foreach (DataRow dr_t in dt.Rows)
                {
                    result = Extension.deleteorder(dr_t["CODE"] + "");
                }
            }
            return result;
        }

        //重新编写国内业务的订单逻辑 by panhuaguo 2016-03-10
        public string loadform()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ordercode = Request["ordercode"];
            DataTable dt;
            string data1 = "{}", data2 = "{}", data3 = "{}", data4 = "{}", ASSOCIATENO = "", filedata1 = "[]", filedata2 = "[]";
            string code1 = "", code2 = "", code3 = "", code4 = "";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string ordercodes = "";//存储所有的订单号
            string bgsb_unit = "";
            string bjsb_unit = "";

            if (string.IsNullOrEmpty(ordercode))//如果订单号为空、即新增的时候
            {
                string sql = "select * from base_company where CODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "' AND ENABLED=1 AND ROWNUM=1";//根据海关的10位编码查询申报单位
                dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    bgsb_unit = dt.Rows[0]["NAME"] + "";
                }
                sql = "select * from base_company where INSPCODE='" + json_user.Value<string>("CUSTOMERCIQCODE") + "' AND ENABLED=1 AND ROWNUM=1";//根据海关的10位编码查询申报单位
                dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    bjsb_unit = dt.Rows[0]["NAME"] + "";
                }
                string repunitcode = bgsb_unit + "(" + json_user.Value<string>("CUSTOMERHSCODE") + ")";
                string inspunitcode = bjsb_unit + "(" + json_user.Value<string>("CUSTOMERCIQCODE") + ")";
                string result = "{STATUS:1,REPUNITCODE:'" + repunitcode + "' ,INSPUNITCODE:'" + inspunitcode + "'}";
                return "{data1:" + result + ",data2:" + result + ",data3:" + result + ",data4:" + result + ",filedata1:" + filedata1 + ",filedata2:" + filedata2 + "}";

            }
            else
            {
                string sql_pre = @"select ID,CODE,BUSITYPE,CUSNO,BUSIUNITCODE,CONTRACTNO,TOTALNO,DIVIDENO,TURNPRENO,CLEARANCENO,WOODPACKINGID,
                      LAWCONDITION,GOODSNUM,GOODSWEIGHT,REGNO,REPWAYID,CUSTOMDISTRICTCODE,PORTCODE,
                      REPUNITNAME||'('||REPUNITCODE||')' REPUNITCODE,INSPUNITNAME||'('||INSPUNITCODE||')' INSPUNITCODE,REMARK,
                      CUSTOMERCODE,CREATEUSERID,CREATETIME,SUBMITUSERID,SUBMITTIME,CSACCEPTUSERID,STATUS,ENTRUSTREQUEST, ENTRUSTTYPEID,
                      MANIFEST,SHIPNAME,FILGHTNO,LADINGBILLNO,PRIORITY,TRADEWAYCODES,DECLCARNO,CLEARUNIT,BUSIKIND,DECLWAY,ORDERWAY,PACKKIND,
                      GOODSGW,GOODSNW,ARRIVEDNO,LANDREGNO,CSREQUEST,CLEARREMARK,CSID,CSACCEPTTIME,CSSUBMITTIME,MOID,MOACCEPTTIME,COID,
                      COACCEPTTIME,ISINVALID,FIRSTLADINGBILLNO,SECONDLADINGBILLNO,GOODSTYPEID,CONTAINERNO,SECONDTRANSIT,ASSOCIATENO,CORRESPONDNO,EXTRASTATUS,INTERNALTYPE,
                      ASSOCIATECUSTOMSNO,BUSIUNITNAME,REPUNITNAME,INSPUNITNAME,CUSTOMERNAME,CREATEUSERNAME,
                      SUBMITUSERNAME,CSNAME,MONAME,CONAME,CLEARUNITNAME,FILINGNUMBER,ATTRIBUTION,TRADEWAYCODES1,
                      CSSTARTTIME,CSTIME,SETNUM,PAPERNUM,MOACCEPTUSERID,COACCEPTUSERID,ASSOCIATEPEDECLNO,ISPAUSE,
                      ASSOCIATETRADEWAY,DECLSTATUS,INSPSTATUS,DECLSETNUM,INSPSETNUM,DECLSHEETNUM,INSPSHEETNUM,PREDECLSETNUM,
                      PREINSPSETNUM,MOSTARTTIME,MOENDTIME,MOTIME,COSTARTTIME,COENDTIME,COTIME,DECLPAUSE,INSPPAUSE,PORTNAME,
                      CUSTOMDISTRICTNAME,SUBMITUSERPHONE,CSPHONE,YWBH,FINISHTIME,COBACK,PAUSENUM,MOACCEPTUSERNAME,COACCEPTUSERNAME,
                      CSACCEPTUSERNAME,INVALIDREASON,CSISBACK,MOISBACK,COISBACK,INOUTTYPE,MOSUBMITTIME,
                      BUSISHORTNAME,BUSISHORTCODE, COSUBMITTIME,IETYPE,CSSUBMITUSERID,CSSUBMITUSERNAME,SPECIALRELATIONSHIP,
                      PRICEIMPACT,PAYPOYALTIES,PREISPAUSE from LIST_ORDER t";
                string sql = sql_pre + " where CODE = '" + ordercode + "'";
                DataTable dt1 = DBMgr.GetDataTable(sql);
                if (!string.IsNullOrEmpty(dt1.Rows[0]["CORRESPONDNO"] + ""))//如果存在多单关联号 多单关联号一定在主订单组里面
                {
                    string correspondno = dt1.Rows[0]["CORRESPONDNO"] + "";
                    ASSOCIATENO = correspondno.Replace("GF", "GL");//通过多单关联号，如果两单关联号存在的话，可以通过这种方式计算出来

                    sql = sql_pre + " where t.BUSITYPE = '41' and ASSOCIATENO='" + ASSOCIATENO + "' and CORRESPONDNO='" + correspondno + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)//有可能主订单组里面只有出没有进，所以要判断一下
                    {
                        data1 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code1 = dt.Rows[0]["CODE"] + "";
                        ordercodes += string.IsNullOrEmpty(ordercodes) ? code1 : "," + code1;
                    }
                    sql = sql_pre + " where BUSITYPE = '40' and ASSOCIATENO='" + ASSOCIATENO + "' and CORRESPONDNO='" + correspondno + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        data2 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code2 = dt.Rows[0]["CODE"] + "";
                        ordercodes += string.IsNullOrEmpty(ordercodes) ? code2 : "," + code2;
                    }
                    sql = sql_pre + " where BUSITYPE = '41' and ASSOCIATENO!='" + ASSOCIATENO + "' and CORRESPONDNO='" + correspondno + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        data3 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code3 = dt.Rows[0]["CODE"] + "";
                        ordercodes += string.IsNullOrEmpty(ordercodes) ? code3 : "," + code3;
                    }
                    sql = sql_pre + " where BUSITYPE = '40' and ASSOCIATENO!='" + ASSOCIATENO + "' and CORRESPONDNO='" + correspondno + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        data4 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code4 = dt.Rows[0]["CODE"] + "";
                        ordercodes += string.IsNullOrEmpty(ordercodes) ? code4 : "," + code4;
                    }
                }
                else if (!string.IsNullOrEmpty(dt1.Rows[0]["ASSOCIATENO"] + ""))//如果存在两单关联号
                {
                    ASSOCIATENO = dt1.Rows[0]["ASSOCIATENO"] + "";
                    sql = sql_pre + " where BUSITYPE = '41' and ASSOCIATENO='" + ASSOCIATENO + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        data1 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code1 = dt.Rows[0]["CODE"] + "";
                    }
                    sql = sql_pre + " where BUSITYPE = '40' and ASSOCIATENO='" + ASSOCIATENO + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        data2 = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                        code2 = dt.Rows[0]["CODE"] + "";
                    }
                }
                //获取随附文件信息
                string where = "";
                sql = @"select * from list_attachment where (filetype=44 or filetype=57 or filetype=58) and (abolishstatus is null or abolishstatus=0)";

                if (!string.IsNullOrEmpty(code1 + code2))
                {
                    where += " and instr('" + code1 + code2 + "',ordercode)>0";
                }
                filedata1 = !string.IsNullOrEmpty(where) ? JsonConvert.SerializeObject(DBMgr.GetDataTable(sql + where), iso) : "[]";

                where = "";
                sql = @"select * from list_attachment where (filetype=44 or filetype=57 or filetype=58) and (abolishstatus is null or abolishstatus=0)";
                if (!string.IsNullOrEmpty(code3 + code4))
                {
                    where += " and instr('" + code3 + code4 + "',ordercode)>0";
                }
                filedata2 = !string.IsNullOrEmpty(where) ? JsonConvert.SerializeObject(DBMgr.GetDataTable(sql + where), iso) : "[]";
                return "{ORDERCODES:'" + ordercodes + "',data1:" + data1 + ",data2:" + data2 + ",filedata1:" + filedata1 + ",data3:" + data3 + ",data4:" + data4 + ",filedata2:" + filedata2 + "}";
            }
        }

        //上传文件按钮除了基本的控制外，还有一种情形就是当后台开启上传权限的数量，即使提交还是可以上传的16050508667
        public string AdditionFile()
        {
            DataTable dt;
            string sql = @"select * from LIST_ORDER t where t.CODE='" + Request["ORDERCODE"] + "'";
            dt = DBMgr.GetDataTable(sql);
            if (!string.IsNullOrEmpty(dt.Rows[0]["CORRESPONDNO"] + ""))//如果存在多单关联号 多单关联号一定在主订单组里面
            {
                sql = @"select ID from list_fileconfig where STATUS = 0 and ordercode in (select CODE FROM LIST_ORDER WHERE CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "')";
            }
            else if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"] + ""))//如果存在两单关联号
            {
                sql = @"select ID from list_fileconfig where STATUS = 0 and ordercode in (select CODE FROM LIST_ORDER WHERE ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "')";
            }
            dt = DBMgr.GetDataTable(sql);
            return "{result:" + dt.Rows.Count + "}";
        }

        public ActionResult Upload_WebServer(int? chunk, string name)
        {
            var fileUpload = Request.Files[0];
            var uploadPath = Server.MapPath("/FileUpload/file");
            chunk = chunk ?? 0;
            using (var fs = new FileStream(Path.Combine(uploadPath, name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
            return Content("chunk uploaded", "text/plain");
        }

        //撤单之前再次判断数据库状态，以防止页面为刷新
        public string GetOrder_ByCode(string ordercode)
        {
            string data = "{}";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = "select * from list_order where CODE = '" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)//有可能主订单组里面只有出没有进，所以要判断一下
            {
                data = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
            }
            return data;
        }

        //撤单操作
        public string Order_Cancel_Submit(string ordercode)
        {
            try
            {
                string sql = "select CORRESPONDNO,ASSOCIATENO from list_order where CODE = '" + ordercode + "'";
                DataTable dt = DBMgr.GetDataTable(sql);

                string sql_tmp = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["CORRESPONDNO"] + ""))//如果存在多单关联号
                {
                    sql_tmp = "select * from list_order where CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "'";
                    sql = @"update list_order set STATUS=10,SUBMITTIME=NULL,SUBMITUSERNAME=NULL,SUBMITUSERID=NULL,SUBMITUSERPHONE=NULL 
                          where CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "'";
                }
                else if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"] + ""))
                {
                    sql_tmp = "select * from list_order where ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                    sql = @"update list_order set STATUS=10,SUBMITTIME=NULL,SUBMITUSERNAME=NULL,SUBMITUSERID=NULL,SUBMITUSERPHONE=NULL 
                          where ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                }

                DBMgr.ExecuteNonQuery(sql);

                //删除订单状态变更日志和提交时产生的预配信息
                dt = DBMgr.GetDataTable(sql_tmp);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "delete from list_cspond where ordercode='" + dr["CODE"] + "'";
                    DBMgr.ExecuteNonQuery(sql);

                    sql = "delete from list_times where CODE='" + dr["CODE"] + "' AND STATUS=15";
                    DBMgr.ExecuteNonQuery(sql);
                    
                }
                return "{success:true}";
            }
            catch (Exception ex)
            {
                return "{success:false}";
            }
        }

        public void Del_Order(string del_ordercodes)
        {
            System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
            string UserName = ConfigurationManager.AppSettings["FTPUserName"];
            string Password = ConfigurationManager.AppSettings["FTPPassword"];
            FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
            string[] code_array = del_ordercodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string sql_tmp = "";
            DataTable dt_tmp;
            foreach (string code in code_array)
            {
                sql_tmp = @"select * from LIST_ATTACHMENT where ordercode='" + code + "'";//删除文件表记录及其附件
                dt_tmp = DBMgr.GetDataTable(sql_tmp);
                foreach (DataRow dr in dt_tmp.Rows)
                {
                    ftp.DeleteFile(dr["FILENAME"] + "");
                    sql_tmp = @"delete from LIST_ATTACHMENT where ID='" + dr["ID"] + "'";
                    DBMgr.ExecuteNonQuery(sql_tmp);
                }
                sql_tmp = @"delete from list_times where CODE='" + code + "'";//删除订单状态变更记录
                DBMgr.ExecuteNonQuery(sql_tmp);
                sql_tmp = "delete from list_order where CODE='" + code + "'";
                DBMgr.ExecuteNonQuery(sql_tmp);
            }
        }

        public string Save()
        {
            DataTable dt;
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            JObject json_head1 = (JObject)JsonConvert.DeserializeObject(Request["data_head1"]);
            JObject json1 = (JObject)JsonConvert.DeserializeObject(Request["data1"]);
            JObject json2 = (JObject)JsonConvert.DeserializeObject(Request["data2"]);
            JObject json_head2 = (JObject)JsonConvert.DeserializeObject(Request["data_head2"]);
            JObject json3 = (JObject)JsonConvert.DeserializeObject(Request["data3"]);
            JObject json4 = (JObject)JsonConvert.DeserializeObject(Request["data4"]);
            //判定上传文件开关开启后是否有上传文件  如果上传过了，关闭开关
            if (!string.IsNullOrEmpty(json_head1.Value<string>("ADDITION")))
            {
                string sql_1 = @"select * from LIST_ORDER t where t.CODE='" + json_head1.Value<string>("ADDITION") + "'";
                dt = DBMgr.GetDataTable(sql_1);
                if (!string.IsNullOrEmpty(dt.Rows[0]["CORRESPONDNO"] + ""))//如果存在多单关联号 多单关联号一定在主订单组里面
                {
                    sql_1 = @"update list_fileconfig set STATUS=1 where STATUS = 0 and ordercode in (select CODE FROM LIST_ORDER WHERE CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "')";
                }
                else if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"] + ""))//如果存在两单关联号
                {
                    sql_1 = @"update list_fileconfig set STATUS=1 where STATUS = 0 and ordercode in (select CODE FROM LIST_ORDER WHERE ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "')";
                }
                DBMgr.ExecuteNonQuery(sql_1);
            }
            //关联订单贸易方式 在前端没有
            json1.Add("ASSOCIATETRADEWAY", ""); json2.Add("ASSOCIATETRADEWAY", ""); json3.Add("ASSOCIATETRADEWAY", ""); json4.Add("ASSOCIATETRADEWAY", "");

            if (Request["action"] + "" == "submit")
            {
                json1.Remove("STATUS"); json2.Remove("STATUS"); json3.Remove("STATUS"); json4.Remove("STATUS");
                json_head1.Remove("SUBMITTIME"); json_head2.Remove("SUBMITTIME");//委托时间
                json_head1.Remove("SUBMITUSERNAME"); json_head2.Remove("SUBMITUSERNAME"); //委托人NAME 
                json_head1.Remove("SUBMITUSERID"); json_head2.Remove("SUBMITUSERID"); //委托人ID
                json_head1.Remove("SUBMITUSERPHONE"); json_head2.Remove("SUBMITUSERPHONE"); //委托人电话SUBMITUSERPHONE               

                json1.Add("STATUS", 15); json2.Add("STATUS", 15); json3.Add("STATUS", 15); json4.Add("STATUS", 15);
                json_head1.Add("SUBMITTIME", "sysdate"); json_head2.Add("SUBMITTIME", "sysdate");
                json_head1.Add("SUBMITUSERNAME", json_user.Value<string>("REALNAME")); json_head2.Add("SUBMITUSERNAME", json_user.Value<string>("REALNAME"));
                json_head1.Add("SUBMITUSERID", json_user.Value<string>("ID")); json_head2.Add("SUBMITUSERID", json_user.Value<string>("ID"));
                json_head1.Add("SUBMITUSERPHONE", json_user.Value<string>("TELEPHONE") + "|" + json_user.Value<string>("MOBILEPHONE")); json_head2.Add("SUBMITUSERPHONE", json_user.Value<string>("TELEPHONE") + "|" + json_user.Value<string>("MOBILEPHONE"));
            }
            else
            {
                if (string.IsNullOrEmpty(json_head1.Value<string>("SUBMITTIME")))
                {
                    json_head1.Remove("SUBMITTIME"); json_head2.Remove("SUBMITTIME");//委托时间  因为该字段需要取ORACLE的时间，而非系统时间 所以需要特殊处理,格式化时并没有加引号
                    json_head1.Add("SUBMITTIME", "null"); json_head2.Add("SUBMITTIME", "null");
                }
                else//有可能提交以后再对部分字段进行修改后保存
                {
                    string submittime1 = json_head1.Value<string>("SUBMITTIME");
                    string submittime2 = json_head2.Value<string>("SUBMITTIME");
                    json_head1.Remove("SUBMITTIME"); json_head2.Remove("SUBMITTIME");
                    json_head1.Add("SUBMITTIME", "to_date('" + submittime1 + "','yyyy-MM-dd HH24:mi:ss')");
                    json_head2.Add("SUBMITTIME", "to_date('" + submittime2 + "','yyyy-MM-dd HH24:mi:ss')");
                }
            }

            string code1 = ""; string code2 = ""; string code3 = ""; string code4 = "";
            string AssociateNo = ""; string AssociateNo2 = "";//两单关联号
            string CorrespondNo = "";//多单关联号 
            string original_codes = json_head1.Value<string>("ORDERCODES");//旧的订单号
            string insert_sql = ""; string update_sql = ""; string sql = ""; string ordercode = ""; 

            if (json_head1.Value<string>("IETYPE") == "进/出口业务")//根据顶部表单的进出类型生成或者获取订单编号
            {
                code1 = string.IsNullOrEmpty(json1.Value<string>("CODE")) ? Extension.getOrderCode() : json1.Value<string>("CODE");
                AssociateNo = "GL" + code1;
                code2 = string.IsNullOrEmpty(json2.Value<string>("CODE")) ? Extension.getOrderCode() : json2.Value<string>("CODE");

                json1.Remove("ASSOCIATETRADEWAY"); json2.Remove("ASSOCIATETRADEWAY");
                json1.Add("ASSOCIATETRADEWAY", json2.Value<string>("TRADEWAYCODES"));   //两单关联时提交委托需要保存对方订单的贸易方式
                json2.Add("ASSOCIATETRADEWAY", json1.Value<string>("TRADEWAYCODES"));

                original_codes = original_codes.Replace(code1, "").Replace(code2, "");
            }
            if (json_head1.Value<string>("IETYPE") == "仅进口")
            {
                code1 = string.IsNullOrEmpty(json1.Value<string>("CODE")) ? Extension.getOrderCode() : json1.Value<string>("CODE");
                AssociateNo = "GL" + code1;
                json2 = new JObject();//防止前端切换了进出口类型后，表单并没有销毁，但是数据送到了后台
                original_codes = original_codes.Replace(code1, "");
            }
            if (json_head1.Value<string>("IETYPE") == "仅出口")
            {
                json1 = new JObject();//防止前端切换了进出口类型后，表单并没有销毁，但是数据送到了后台
                code2 = string.IsNullOrEmpty(json2.Value<string>("CODE")) ? Extension.getOrderCode() : json2.Value<string>("CODE");
                AssociateNo = "GL" + code2;
                original_codes = original_codes.Replace(code2, "");
            }
            if (json_head2.Value<string>("IETYPE") == "进/出口业务")
            {
                code3 = string.IsNullOrEmpty(json3.Value<string>("CODE")) ? Extension.getOrderCode() : json3.Value<string>("CODE");
                AssociateNo2 = "GL" + code3;
                code4 = string.IsNullOrEmpty(json4.Value<string>("CODE")) ? Extension.getOrderCode() : json4.Value<string>("CODE");

                json3.Remove("ASSOCIATETRADEWAY"); json4.Remove("ASSOCIATETRADEWAY");
                json3.Add("ASSOCIATETRADEWAY", json4.Value<string>("TRADEWAYCODES"));   //两单关联时提交委托需要保存对方订单的贸易方式
                json4.Add("ASSOCIATETRADEWAY", json3.Value<string>("TRADEWAYCODES"));

                original_codes = original_codes.Replace(code3, "").Replace(code4, "");
            }
            if (json_head2.Value<string>("IETYPE") == "仅进口")
            {
                code3 = string.IsNullOrEmpty(json3.Value<string>("CODE")) ? Extension.getOrderCode() : json3.Value<string>("CODE");
                AssociateNo2 = "GL" + code3;
                json4 = new JObject();//防止前端切换了进出口类型后，表单并没有销毁，但是数据送到了后台
                original_codes = original_codes.Replace(code3, "");
            }
            if (json_head2.Value<string>("IETYPE") == "仅出口")
            {
                json3 = new JObject();//防止前端切换了进出口类型后，表单并没有销毁，但是数据送到了后台
                code4 = string.IsNullOrEmpty(json4.Value<string>("CODE")) ? Extension.getOrderCode() : json4.Value<string>("CODE");
                AssociateNo2 = "GL" + code4;
                original_codes = original_codes.Replace(code4, "");
            }
            if ((!string.IsNullOrEmpty(code1) || !string.IsNullOrEmpty(code2)) && (!string.IsNullOrEmpty(code3) || !string.IsNullOrEmpty(code4)))
            {
                CorrespondNo = !string.IsNullOrEmpty(code1) ? "GF" + code1 : "GF" + code2;
            }

            //比对新的订单号和前端已经存在的订单号 删除已经废弃的订单号及其相关表记录           
            Del_Order(original_codes);
                       
            insert_sql = @"INSERT INTO LIST_ORDER (ID,BUSITYPE,ASSOCIATEPEDECLNO,CODE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO,GOODSNUM,CLEARANCENO,
                                                LAWCONDITION,ENTRUSTTYPEID,REPWAYID,CUSTOMDISTRICTCODE,CUSTOMDISTRICTNAME,REPUNITCODE,REPUNITNAME,DECLWAY,INSPUNITCODE,
                                                INSPUNITNAME,ENTRUSTREQUEST,CREATEUSERID,CREATEUSERNAME,STATUS,CUSTOMERCODE,CUSTOMERNAME,TRADEWAYCODES,
                                                AssociateNo,CorrespondNo,PACKKIND,GOODSGW,GOODSNW,FILINGNUMBER,TRADEWAYCODES1,IETYPE,SPECIALRELATIONSHIP,PRICEIMPACT,PAYPOYALTIES,BUSISHORTCODE,BUSISHORTNAME,
                                                SUBMITTIME,SUBMITUSERNAME,SUBMITUSERID,SUBMITUSERPHONE,ASSOCIATETRADEWAY,BUSIKIND,ORDERWAY,SCENEDECLAREID,SCENEINSPECTID) 
                                                VALUES (LIST_ORDER_id.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',
                                                '{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}',
                                                '{33}','{34}','{35}','{36}','{37}','{38}',{39},'{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}')";           
           
            update_sql = @"update LIST_ORDER  set ASSOCIATEPEDECLNO='{0}',CUSNO='{1}',BUSIUNITCODE='{2}',BUSIUNITNAME='{3}',CONTRACTNO='{4}',GOODSNUM='{5}',
                                        CLEARANCENO='{6}',LAWCONDITION='{7}',ENTRUSTTYPEID='{8}' ,REPWAYID='{9}',CUSTOMDISTRICTCODE='{10}',CUSTOMDISTRICTNAME='{11}',
                                        REPUNITCODE='{12}',REPUNITNAME='{13}',DECLWAY='{14}',INSPUNITCODE='{15}',INSPUNITNAME='{16}',ENTRUSTREQUEST='{17}',TRADEWAYCODES='{18}',
                                        ASSOCIATENO='{19}',CORRESPONDNO='{20}',PACKKIND='{21}',GOODSGW='{22}',GOODSNW='{23}',FILINGNUMBER='{24}',TRADEWAYCODES1='{25}',
                                        IETYPE='{26}',SPECIALRELATIONSHIP='{27}',PRICEIMPACT='{28}',PAYPOYALTIES='{29}',BUSISHORTCODE='{30}',BUSISHORTNAME='{31}',STATUS='{32}',
                                SUBMITTIME={33},SUBMITUSERNAME='{34}',SUBMITUSERID='{35}',SUBMITUSERPHONE='{36}',ASSOCIATETRADEWAY='{37}',BUSIKIND='{38}',ORDERWAY='{39}' where CODE='{40}'";
            
            string exe_desc = "";//订单保存时记录各订单的执行情况
            int order_res = 1;
            if (!string.IsNullOrEmpty(code1))//如果code1不为空 
            {
                if (string.IsNullOrEmpty(json1.Value<string>("CODE")))//新增
                {
                    sql = string.Format(insert_sql, "41", json1.Value<string>("ASSOCIATEPEDECLNO"), code1, json1.Value<string>("CUSNO"), json1.Value<string>("BUSIUNITCODE"),
                         json1.Value<string>("BUSIUNITNAME"), json1.Value<string>("CONTRACTNO"), json1.Value<string>("GOODSNUM"), json1.Value<string>("CLEARANCENO"),
                         GetChk(json1.Value<string>("LAWCONDITION")), json1.Value<string>("ENTRUSTTYPEID"), json_head1.Value<string>("REPWAYID"),
                         json_head1.Value<string>("CUSTOMDISTRICTCODE"), json_head1.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json1.Value<string>("REPUNITCODE")),
                         GetName(json1.Value<string>("REPUNITCODE")), json1.Value<string>("DECLWAY"), GetCode(json1.Value<string>("INSPUNITCODE")),
                         GetName(json1.Value<string>("INSPUNITCODE")), json1.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                         json1.Value<string>("STATUS"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json1.Value<string>("TRADEWAYCODES"),
                         AssociateNo, CorrespondNo, json1.Value<string>("PACKKIND"), json1.Value<string>("GOODSGW"), json1.Value<string>("GOODSNW"),
                         json1.Value<string>("FILINGNUMBER"), json1.Value<string>("TRADEWAYCODES1"), json_head1.Value<string>("IETYPE"), GetChk(json1.Value<string>("SPECIALRELATIONSHIP")),
                         GetChk(json1.Value<string>("PRICEIMPACT")), GetChk(json1.Value<string>("PAYPOYALTIES")), json1.Value<string>("BUSISHORTCODE"), json1.Value<string>("BUSISHORTNAME"),
                         json_head1.Value<string>("SUBMITTIME"), json_head1.Value<string>("SUBMITUSERNAME"), json_head1.Value<string>("SUBMITUSERID"), json_head1.Value<string>("SUBMITUSERPHONE"), json1.Value<string>("ASSOCIATETRADEWAY"),
                         "002", "1", json_user.Value<string>("SCENEDECLAREID"), json_user.Value<string>("SCENEINSPECTID")
                         );
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code1;
                }
                else
                {
                    sql = string.Format(update_sql, json1.Value<string>("ASSOCIATEPEDECLNO"), json1.Value<string>("CUSNO"), json1.Value<string>("BUSIUNITCODE"),
                         json1.Value<string>("BUSIUNITNAME"), json1.Value<string>("CONTRACTNO"), json1.Value<string>("GOODSNUM"), json1.Value<string>("CLEARANCENO"),
                         GetChk(json1.Value<string>("LAWCONDITION")), json1.Value<string>("ENTRUSTTYPEID"), json_head1.Value<string>("REPWAYID"),
                         json_head1.Value<string>("CUSTOMDISTRICTCODE"), json_head1.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json1.Value<string>("REPUNITCODE")),
                         GetName(json1.Value<string>("REPUNITCODE")), json1.Value<string>("DECLWAY"), GetCode(json1.Value<string>("INSPUNITCODE")), GetName(json1.Value<string>("INSPUNITCODE")),
                         json1.Value<string>("ENTRUSTREQUEST"), json1.Value<string>("TRADEWAYCODES"), AssociateNo, CorrespondNo, json1.Value<string>("PACKKIND"),
                          json1.Value<string>("GOODSGW"), json1.Value<string>("GOODSNW"), json1.Value<string>("FILINGNUMBER"), json1.Value<string>("TRADEWAYCODES1"),
                         json_head1.Value<string>("IETYPE"), GetChk(json1.Value<string>("SPECIALRELATIONSHIP")), GetChk(json1.Value<string>("PRICEIMPACT")),
                         GetChk(json1.Value<string>("PAYPOYALTIES")), json1.Value<string>("BUSISHORTCODE"), json1.Value<string>("BUSISHORTNAME"), json1.Value<string>("STATUS"),
                         json_head1.Value<string>("SUBMITTIME"), json_head1.Value<string>("SUBMITUSERNAME"), json_head1.Value<string>("SUBMITUSERID"), json_head1.Value<string>("SUBMITUSERPHONE"), json1.Value<string>("ASSOCIATETRADEWAY"), "002", "1", code1);
                    if (json1.Value<Int32>("STATUS") >= 20)  //当业务状态为订单已受理对空白字段的修改需要记录到字段修改记录表
                    {
                        Extension.Insert_FieldUpdate_History(code1, json1, json_user, "41");
                    }
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code1;
                }
                if (order_res == 0)
                {
                    exe_desc = "订单：" + code1 + "保存失败!<br />";
                }
                if (Request["action"] + "" == "submit")
                {
                    Extension.add_cspond(json1.Value<string>("BUSIUNITCODE"), "41", code1, AssociateNo, CorrespondNo, json_user);
                }
                Extension.add_list_time(json1.Value<Int32>("STATUS"), code1, json_user);
            }

            if (!string.IsNullOrEmpty(code2))
            {
                if (string.IsNullOrEmpty(json2.Value<string>("CODE")))//新增
                {
                    sql = string.Format(insert_sql, "40", json2.Value<string>("ASSOCIATEPEDECLNO"), code2, json2.Value<string>("CUSNO"), json2.Value<string>("BUSIUNITCODE"),
                         json2.Value<string>("BUSIUNITNAME"), json2.Value<string>("CONTRACTNO"), json2.Value<string>("GOODSNUM"), json2.Value<string>("CLEARANCENO"),
                         GetChk(json2.Value<string>("LAWCONDITION")), json2.Value<string>("ENTRUSTTYPEID"), json_head1.Value<string>("REPWAYID"),
                         json_head1.Value<string>("CUSTOMDISTRICTCODE"), json_head1.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json2.Value<string>("REPUNITCODE")),
                         GetName(json2.Value<string>("REPUNITCODE")), json2.Value<string>("DECLWAY"), GetCode(json2.Value<string>("INSPUNITCODE")),
                         GetName(json2.Value<string>("INSPUNITCODE")), json2.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                         json2.Value<string>("STATUS"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json2.Value<string>("TRADEWAYCODES"),
                         AssociateNo, CorrespondNo, json2.Value<string>("PACKKIND"), json2.Value<string>("GOODSGW"), json2.Value<string>("GOODSNW"),
                         json2.Value<string>("FILINGNUMBER"), json2.Value<string>("TRADEWAYCODES1"), json_head1.Value<string>("IETYPE"), GetChk(json2.Value<string>("SPECIALRELATIONSHIP")),
                         GetChk(json2.Value<string>("PRICEIMPACT")), GetChk(json2.Value<string>("PAYPOYALTIES")), json2.Value<string>("BUSISHORTCODE"), json2.Value<string>("BUSISHORTNAME"),
                         json_head1.Value<string>("SUBMITTIME"), json_head1.Value<string>("SUBMITUSERNAME"), json_head1.Value<string>("SUBMITUSERID"), json_head1.Value<string>("SUBMITUSERPHONE"), json2.Value<string>("ASSOCIATETRADEWAY"),
                         "002", "1", json_user.Value<string>("SCENEDECLAREID"), json_user.Value<string>("SCENEINSPECTID")
                         );
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code2;
                }
                else
                {
                    sql = string.Format(update_sql, json2.Value<string>("ASSOCIATEPEDECLNO"), json2.Value<string>("CUSNO"), json2.Value<string>("BUSIUNITCODE"),
                         json2.Value<string>("BUSIUNITNAME"), json2.Value<string>("CONTRACTNO"), json2.Value<string>("GOODSNUM"), json2.Value<string>("CLEARANCENO"),
                         GetChk(json2.Value<string>("LAWCONDITION")), json2.Value<string>("ENTRUSTTYPEID"), json_head1.Value<string>("REPWAYID"),
                         json_head1.Value<string>("CUSTOMDISTRICTCODE"), json_head1.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json2.Value<string>("REPUNITCODE")),
                         GetName(json2.Value<string>("REPUNITCODE")), json2.Value<string>("DECLWAY"), GetCode(json2.Value<string>("INSPUNITCODE")), GetName(json2.Value<string>("INSPUNITCODE")),
                         json2.Value<string>("ENTRUSTREQUEST"), json2.Value<string>("TRADEWAYCODES"), AssociateNo, CorrespondNo, json2.Value<string>("PACKKIND"),
                         json2.Value<string>("GOODSGW"), json2.Value<string>("GOODSNW"), json2.Value<string>("FILINGNUMBER"), json2.Value<string>("TRADEWAYCODES1"),
                         json_head1.Value<string>("IETYPE"), GetChk(json2.Value<string>("SPECIALRELATIONSHIP")), GetChk(json2.Value<string>("PRICEIMPACT")),
                         GetChk(json2.Value<string>("PAYPOYALTIES")), json2.Value<string>("BUSISHORTCODE"), json2.Value<string>("BUSISHORTNAME"), json2.Value<string>("STATUS"),
                         json_head1.Value<string>("SUBMITTIME"), json_head1.Value<string>("SUBMITUSERNAME"), json_head1.Value<string>("SUBMITUSERID"), json_head1.Value<string>("SUBMITUSERPHONE"), json2.Value<string>("ASSOCIATETRADEWAY"), "002", "1", code2);
                    if (json2.Value<Int32>("STATUS") >= 20)  //当业务状态为订单已受理对空白字段的修改需要记录到字段修改记录表
                    {
                        Extension.Insert_FieldUpdate_History(code2, json2, json_user, "40");
                    }
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code2;
                }
                if (order_res == 0)
                {
                    exe_desc = "订单：" + code2 + "保存失败!<br />";
                }
                if (Request["action"] + "" == "submit")
                {
                    Extension.add_cspond(json2.Value<string>("BUSIUNITCODE"), "40", code2, AssociateNo, CorrespondNo, json_user);
                }
                Extension.add_list_time(json2.Value<Int32>("STATUS"), code2, json_user);
            }
            if (!string.IsNullOrEmpty(code3))
            {
                if (string.IsNullOrEmpty(json3.Value<string>("CODE")))//新增
                {
                    sql = string.Format(insert_sql, "41", json3.Value<string>("ASSOCIATEPEDECLNO"), code3, json3.Value<string>("CUSNO"), json3.Value<string>("BUSIUNITCODE"),
                         json3.Value<string>("BUSIUNITNAME"), json3.Value<string>("CONTRACTNO"), json3.Value<string>("GOODSNUM"), json3.Value<string>("CLEARANCENO"),
                         GetChk(json3.Value<string>("LAWCONDITION")), json3.Value<string>("ENTRUSTTYPEID"), json_head1.Value<string>("REPWAYID"),
                         json_head2.Value<string>("CUSTOMDISTRICTCODE"), json_head2.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json3.Value<string>("REPUNITCODE")),
                         GetName(json3.Value<string>("REPUNITCODE")), json3.Value<string>("DECLWAY"), GetCode(json3.Value<string>("INSPUNITCODE")),
                         GetName(json3.Value<string>("INSPUNITCODE")), json3.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                         json3.Value<string>("STATUS"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json3.Value<string>("TRADEWAYCODES"),
                         AssociateNo2, CorrespondNo, json3.Value<string>("PACKKIND"), json3.Value<string>("GOODSGW"), json3.Value<string>("GOODSNW"),
                         json3.Value<string>("FILINGNUMBER"), json3.Value<string>("TRADEWAYCODES1"), json_head2.Value<string>("IETYPE"), GetChk(json3.Value<string>("SPECIALRELATIONSHIP")),
                         GetChk(json3.Value<string>("PRICEIMPACT")), GetChk(json3.Value<string>("PAYPOYALTIES")), json3.Value<string>("BUSISHORTCODE"), json3.Value<string>("BUSISHORTNAME"),
                         json_head2.Value<string>("SUBMITTIME"), json_head2.Value<string>("SUBMITUSERNAME"), json_head2.Value<string>("SUBMITUSERID"), json_head2.Value<string>("SUBMITUSERPHONE"), json3.Value<string>("ASSOCIATETRADEWAY"),
                         "002", "1", json_user.Value<string>("SCENEDECLAREID"), json_user.Value<string>("SCENEINSPECTID")
                         );
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code3;
                }
                else
                {
                    sql = string.Format(update_sql, json3.Value<string>("ASSOCIATEPEDECLNO"), json3.Value<string>("CUSNO"), json3.Value<string>("BUSIUNITCODE"),
                         json3.Value<string>("BUSIUNITNAME"), json3.Value<string>("CONTRACTNO"), json3.Value<string>("GOODSNUM"), json3.Value<string>("CLEARANCENO"),
                         GetChk(json3.Value<string>("LAWCONDITION")), json3.Value<string>("ENTRUSTTYPEID"), json_head2.Value<string>("REPWAYID"),
                         json_head2.Value<string>("CUSTOMDISTRICTCODE"), json_head2.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json3.Value<string>("REPUNITCODE")),
                         GetName(json3.Value<string>("REPUNITCODE")), json3.Value<string>("DECLWAY"), GetCode(json3.Value<string>("INSPUNITCODE")), GetName(json3.Value<string>("INSPUNITCODE")),
                         json3.Value<string>("ENTRUSTREQUEST"), json3.Value<string>("TRADEWAYCODES"), AssociateNo2, CorrespondNo, json3.Value<string>("PACKKIND"),
                         json3.Value<string>("GOODSGW"), json3.Value<string>("GOODSNW"), json3.Value<string>("FILINGNUMBER"), json3.Value<string>("TRADEWAYCODES1"),
                         json_head2.Value<string>("IETYPE"), GetChk(json3.Value<string>("SPECIALRELATIONSHIP")), GetChk(json3.Value<string>("PRICEIMPACT")),
                         GetChk(json3.Value<string>("PAYPOYALTIES")), json3.Value<string>("BUSISHORTCODE"), json3.Value<string>("BUSISHORTNAME"), json3.Value<string>("STATUS"),
                         json_head2.Value<string>("SUBMITTIME"), json_head2.Value<string>("SUBMITUSERNAME"), json_head2.Value<string>("SUBMITUSERID"), json_head2.Value<string>("SUBMITUSERPHONE"), json3.Value<string>("ASSOCIATETRADEWAY"), "002", "1", code3);
                    if (json3.Value<Int32>("STATUS") >= 20)  //当业务状态为订单已受理对空白字段的修改需要记录到字段修改记录表
                    {
                        Extension.Insert_FieldUpdate_History(code3, json3, json_user, "41");
                    }
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code3;
                }
                if (order_res == 0)
                {
                    exe_desc = "订单：" + code3 + "保存失败!<br />";
                }
                if (Request["action"] + "" == "submit")
                {
                    Extension.add_cspond(json3.Value<string>("BUSIUNITCODE"), "41", code3, AssociateNo2, CorrespondNo, json_user);
                }
                Extension.add_list_time(json3.Value<Int32>("STATUS"), code3, json_user);
            }
            if (!string.IsNullOrEmpty(code4))
            {
                if (string.IsNullOrEmpty(json4.Value<string>("CODE")))//新增
                {
                    sql = string.Format(insert_sql, "40", json4.Value<string>("ASSOCIATEPEDECLNO"), code4, json4.Value<string>("CUSNO"), json4.Value<string>("BUSIUNITCODE"),
                         json4.Value<string>("BUSIUNITNAME"), json4.Value<string>("CONTRACTNO"), json4.Value<string>("GOODSNUM"), json4.Value<string>("CLEARANCENO"),
                         GetChk(json4.Value<string>("LAWCONDITION")), json4.Value<string>("ENTRUSTTYPEID"), json_head2.Value<string>("REPWAYID"),
                         json_head2.Value<string>("CUSTOMDISTRICTCODE"), json_head2.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json4.Value<string>("REPUNITCODE")),
                         GetName(json4.Value<string>("REPUNITCODE")), json4.Value<string>("DECLWAY"), GetCode(json4.Value<string>("INSPUNITCODE")),
                         GetName(json4.Value<string>("INSPUNITCODE")), json4.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                         json4.Value<string>("STATUS"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json4.Value<string>("TRADEWAYCODES"),
                         AssociateNo2, CorrespondNo, json4.Value<string>("PACKKIND"), json4.Value<string>("GOODSGW"), json4.Value<string>("GOODSNW"),
                         json4.Value<string>("FILINGNUMBER"), json4.Value<string>("TRADEWAYCODES1"), json_head2.Value<string>("IETYPE"), GetChk(json4.Value<string>("SPECIALRELATIONSHIP")),
                         GetChk(json4.Value<string>("PRICEIMPACT")), GetChk(json4.Value<string>("PAYPOYALTIES")), json4.Value<string>("BUSISHORTCODE"), json4.Value<string>("BUSISHORTNAME"),
                         json_head2.Value<string>("SUBMITTIME"), json_head2.Value<string>("SUBMITUSERNAME"), json_head2.Value<string>("SUBMITUSERID"), json_head2.Value<string>("SUBMITUSERPHONE"), json4.Value<string>("ASSOCIATETRADEWAY"),
                         "002", "1", json_user.Value<string>("SCENEDECLAREID"), json_user.Value<string>("SCENEINSPECTID")
                         );
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code4;
                }
                else
                {
                    sql = string.Format(update_sql, json4.Value<string>("ASSOCIATEPEDECLNO"), json4.Value<string>("CUSNO"), json4.Value<string>("BUSIUNITCODE"),
                         json4.Value<string>("BUSIUNITNAME"), json4.Value<string>("CONTRACTNO"), json4.Value<string>("GOODSNUM"), json4.Value<string>("CLEARANCENO"),
                         GetChk(json4.Value<string>("LAWCONDITION")), json4.Value<string>("ENTRUSTTYPEID"), json_head2.Value<string>("REPWAYID"),
                         json_head2.Value<string>("CUSTOMDISTRICTCODE"), json_head2.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json4.Value<string>("REPUNITCODE")),
                         GetName(json4.Value<string>("REPUNITCODE")), json4.Value<string>("DECLWAY"), GetCode(json4.Value<string>("INSPUNITCODE")), GetName(json4.Value<string>("INSPUNITCODE")),
                         json4.Value<string>("ENTRUSTREQUEST"), json4.Value<string>("TRADEWAYCODES"), AssociateNo2, CorrespondNo, json4.Value<string>("PACKKIND"),
                         json4.Value<string>("GOODSGW"), json4.Value<string>("GOODSNW"), json4.Value<string>("FILINGNUMBER"), json4.Value<string>("TRADEWAYCODES1"),
                         json_head2.Value<string>("IETYPE"), GetChk(json4.Value<string>("SPECIALRELATIONSHIP")), GetChk(json4.Value<string>("PRICEIMPACT")),
                         GetChk(json4.Value<string>("PAYPOYALTIES")), json4.Value<string>("BUSISHORTCODE"), json4.Value<string>("BUSISHORTNAME"), json4.Value<string>("STATUS"),
                         json_head2.Value<string>("SUBMITTIME"), json_head2.Value<string>("SUBMITUSERNAME"), json_head2.Value<string>("SUBMITUSERID"), json_head2.Value<string>("SUBMITUSERPHONE"), json4.Value<string>("ASSOCIATETRADEWAY"), "002", "1", code4);
                    if (json4.Value<Int32>("STATUS") >= 20)  //当业务状态为订单已受理对空白字段的修改需要记录到字段修改记录表
                    {
                        Extension.Insert_FieldUpdate_History(code4, json4, json_user, "40");
                    }
                    order_res = DBMgr.ExecuteNonQuery(sql);
                    ordercode = code4;
                }
                if (order_res == 0)
                {
                    exe_desc = "订单：" + code4 + "保存失败!<br />";
                }
                if (Request["action"] + "" == "submit")
                {
                    Extension.add_cspond(json4.Value<string>("BUSIUNITCODE"), "40", code4, AssociateNo2, CorrespondNo, json_user);
                }
                Extension.add_list_time(json4.Value<Int32>("STATUS"), code4, json_user);
            }
            string file_data1 = Request["file_data1"] + "";
            string originalids1 = Request["originalids1"] + "";
            Update_Attachment(code1, code2, file_data1, originalids1);

            string file_data2 = Request["file_data2"] + "";
            string originalids2 = Request["originalids2"] + "";
            Update_Attachment(code3, code4, file_data2, originalids2);
            
           
            return "{ordercode:'" + ordercode + "',result:'" + exe_desc + "'}";
        }

        public void Update_Attachment(string code_in, string code_out, string files, string originalids)
        {
            System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
            string UserName = ConfigurationManager.AppSettings["FTPUserName"];
            string Password = ConfigurationManager.AppSettings["FTPPassword"];
            //directory 文件服务器保存的路径目录  2016-4-7 赵燕要求变更随附文件明细ordercode只能保存一个订单号，如果两个订单对应一个文件，则数据库表记录有两条，但文件实际存放路径只有一个
            //考虑后期文件的合并、页码调整 2016-4-21更改为每个订单的文件单独存放
            string code = ""; DataTable dt;
            FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            if (!string.IsNullOrEmpty(files))
            {
                JArray jarry = JsonConvert.DeserializeObject<JArray>(files);
                string sql = "";
                foreach (JObject json in jarry)
                {
                    if (string.IsNullOrEmpty(json.Value<string>("ID")))
                    {
                        code = json.Value<string>("IETYPE") == "仅进口" ? code_in : code_out;
                        string createuserid = json_user.Value<string>("ID");
                        string customercode = json_user.Value<string>("CUSTOMERCODE");
                        string filename = "/" + json.Value<string>("FILETYPE") + "/" + code + "/" + json.Value<string>("ORIGINALNAME");
                        string sizes = json.Value<string>("SIZES");
                        string filetypename = json.Value<string>("FILETYPENAME");
                        string extname = json.Value<string>("ORIGINALNAME").ToString().Substring(json.Value<string>("ORIGINALNAME").ToString().LastIndexOf('.') + 1);
                        if (!string.IsNullOrEmpty(code))//防止只有进口业务，但是上传了出口的文件
                        {
                            sql = @"insert into LIST_ATTACHMENT (id,filename,originalname,filetype,uploadtime,uploaduserid,customercode,ordercode,
                              sizes,filetypename,filesuffix,IETYPE) values(List_Attachment_Id.Nextval,'{0}','{1}','{2}',sysdate,{3},'{4}','{5}','{6}','{7}','{8}','{9}')";
                            sql = string.Format(sql, filename, json.Value<string>("ORIGINALNAME"), json.Value<string>("FILETYPE"), createuserid, customercode, code, sizes, filetypename, extname, json.Value<string>("IETYPE"));
                            DBMgr.ExecuteNonQuery(sql);
                        }
                    }
                    else//如果ID已经存在  说明是已经存在的记录,不需要做任何处理
                    {
                        originalids = originalids.Replace(json.Value<string>("ID") + ",", "");
                    }
                }
                //从数据库和文档库删除在前端移除的记录
                string[] idarray = originalids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in idarray)
                {
                    sql = @"select * from LIST_ATTACHMENT where ID='" + id + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        ftp.DeleteFile(dt.Rows[0]["FILENAME"] + "");
                    }
                    sql = @"delete from LIST_ATTACHMENT where ID='" + id + "'";
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
        }

    }
}
