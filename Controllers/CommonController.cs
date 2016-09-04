using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
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
    [Authorize]
    public class CommonController : Controller
    {
        int totalProperty = 0;

        public ActionResult DeclareList()//报关单管理
        {
            return View();
        }

        //登录后显示菜单栏 by heguiqin 2016-08-25
        public string Header()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t 
            where t.parentid='91a0657f-1939-4528-80aa-91b202a593ab' and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{0}')
            order by sortindex";
            sql = string.Format(sql, json_user.GetValue("ID"));
            DataTable dt1 = DBMgr.GetDataTable(sql);
            string result = "";
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                result += "<li><a href=\"" + dt1.Rows[i]["URL"] + "\">" + dt1.Rows[i]["NAME"] + "</a>";

                sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}'
                and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                sql = string.Format(sql, dt1.Rows[i]["MODULEID"], json_user.GetValue("ID"));
                DataTable dt2 = DBMgr.GetDataTable(sql);
                if (dt2.Rows.Count > 0)
                {
                    result += "<ul>";
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        result += "<li><a href=\"" + dt2.Rows[j]["URL"] + "\">" + dt2.Rows[j]["NAME"] + "</a>";

                        sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}' 
                        and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                        sql = string.Format(sql, dt2.Rows[j]["MODULEID"], json_user.GetValue("ID"));
                        DataTable dt3 = DBMgr.GetDataTable(sql);
                        if (dt3.Rows.Count > 0)
                        {
                            result += "<ul>";
                            for (int k = 0; k < dt3.Rows.Count; k++)
                            {
                                result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + dt3.Rows[k]["NAME"] + "</a></li>";

                            }
                            result += "</ul></li>";
                        }
                        else
                        {
                            result += "</li>";
                        }
                    }
                    result += "</ul></li>";
                }
                else
                {
                    result += "</li>";
                }
            }
            return result;
        }
        //登录后显示顶部当前用户中文名称 by heguiqin 2016-08-25
        public string CurrentUser()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            return json_user.GetValue("REALNAME") + "";
        }

        //订单列表页加载方法 by panhuaguo 2016-08-25
        public string LoadList()
        {

            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and BUSISHORTCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "CUSTOMDISTRICTCODE"://申报关区
                        where += " and CUSTOMDISTRICTCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "PORTCODE"://进口口岸
                        where += " and PORTCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "REPWAYID"://申报方式
                        where += " and REPWAYID='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CODE"://订单编号
                        where += " and instr(CODE,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "CUSNO"://客户编号
                        where += " and instr(CUSNO,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(DIVIDENO,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同发票号
                        where += " and instr(CONTRACTNO,'" + Request["VALUE2"].Trim() + "')>0  ";
                        break;
                    case "MANIFEST"://载货清单号
                        where += " and instr(MANIFEST,'" + Request["VALUE2"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "ddzt"://订单状态                      
                        if ((Request["VALUE3"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and STATUS='1' ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待委托")   //订单待委托 < 已委托
                        {
                            where += " and STATUS < 15 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待受理") //已委托 <= 订单待受理 < 已受理
                        {
                            where += " and STATUS >= 15 and STATUS < 20 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单受理中")  //已受理 <= 订单受理中 < 已交付
                        {
                            where += " and STATUS >= 20 and STATUS < 110 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待交付")  //已委托 <= 订单待交付 < 已交付
                        {
                            where += " and STATUS >= 15 and STATUS < 110 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单已交付")  //订单已交付 = 已交付
                        {
                            where += " and STATUS='110' ";
                        }
                        if ((Request["VALUE3"] + "") == "订单已作废")
                        {
                            where += " and ISINVALID='0' ";
                        }
                        break;
                    case "bgzt":
                        where += " and DECLSTATUS='" + Request["VALUE3"] + "' ";
                        break;
                    case "bjzt":
                        where += " and INSPSTATUS='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME"://委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and SUBMITTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "CSSTARTTIME"://订单开始时间
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and CREATETIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and CREATETIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(Request["VALUE5"]))//判断查询条件5是否有值
            {
                switch (Request["CONDITION5"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and BUSISHORTCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "CUSTOMDISTRICTCODE"://申报关区
                        where += " and CUSTOMDISTRICTCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "PORTCODE"://进口口岸
                        where += " and PORTCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "REPWAYID"://申报方式
                        where += " and REPWAYID='" + Request["VALUE5"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE6"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION6"])
                {
                    case "CODE"://订单编号
                        where += " and instr(CODE,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "CUSNO"://客户编号
                        where += " and instr(CUSNO,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(DIVIDENO,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同发票号
                        where += " and instr(CONTRACTNO,'" + Request["VALUE6"].Trim() + "')>0  ";
                        break;
                    case "MANIFEST"://载货清单号
                        where += " and instr(MANIFEST,'" + Request["VALUE6"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "ddzt"://订单状态
                        //草稿=草稿
                        if ((Request["VALUE7"] + "") == "草稿")
                        {
                            where += " and STATUS='1' ";
                        }
                        //订单待委托 < 已委托
                        if ((Request["VALUE7"] + "") == "订单待委托")
                        {
                            where += " and STATUS < 15 ";
                        }
                        //已委托 <= 订单待受理 < 已受理
                        if ((Request["VALUE7"] + "") == "订单待受理")
                        {
                            where += " and STATUS >= 15 and STATUS < 20 ";
                        }
                        //已受理 <= 订单受理中 < 已交付
                        if ((Request["VALUE7"] + "") == "订单受理中")
                        {
                            where += " and STATUS >= 20 and STATUS < 110 ";
                        }
                        //已委托 <= 订单待交付 < 已交付
                        if ((Request["VALUE7"] + "") == "订单待交付")
                        {
                            where += " and STATUS >= 15 and STATUS < 110 ";
                        }
                        //订单已交付 = 已交付
                        if ((Request["VALUE7"] + "") == "订单已交付")
                        {
                            where += " and STATUS='110' ";
                        }
                        if ((Request["VALUE7"] + "") == "订单已作废")
                        {
                            where += " and ISINVALID='0' ";
                        }
                        break;
                    case "bgzt":
                        where += " and DECLSTATUS='" + Request["VALUE7"] + "' ";
                        break;
                    case "bjzt":
                        where += " and INSPSTATUS='" + Request["VALUE7"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION8"])
            {
                case "SUBMITTIME"://委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and SUBMITTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and SUBMITTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "CSSTARTTIME"://订单开始时间
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and CREATETIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss')' ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and CREATETIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }

            if ((Request["VALUE3"] + "") != "订单已作废" && (Request["VALUE7"] + "") != "订单已作废")//在不查询已作废的订单情形下，皆显示正常的订单
            {
                where += " and ISINVALID='1' ";
            }

            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and CREATEUSERID = " + json_user.Value<string>("ID") + " ";
            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 BUSISHORTNAME,
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select ID, CODE,ENTRUSTTYPEID,CUSNO,PORTCODE,PORTNAME,BUSIUNITNAME,FIRSTLADINGBILLNO,SECONDLADINGBILLNO,
                BUSITYPE,CORRESPONDNO,LADINGBILLNO,ARRIVEDNO,CUSTOMERNAME,CONTRACTNO,TOTALNO,DIVIDENO,TURNPRENO,                
                GOODSNUM || '/'|| GOODSGW  GOODSNUMGOODSNW,GOODSGW,REPWAYID, GOODSWEIGHT,CUSTOMDISTRICTCODE,
                CUSTOMDISTRICTNAME,BUSISHORTNAME,ISINVALID,LAWCONDITION,STATUS,DECLSTATUS,INSPSTATUS,    
                ASSOCIATENO,createtime CREATEDATE,SUBMITTIME from LIST_ORDER where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and customercode='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATEDATE", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        //基础资料 by heguiqin 2016-08-25
        public string Ini_Base_Data()
        {
            IDatabase db = SeRedis.redis.GetDatabase();
            string sql = "";
            string json_sbfs = "[]";//申报方式
            string busitype = Request["busitype"];
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            sql = "select CODE,NAME||'('||CODE||')' NAME from SYS_REPWAY where Enabled=1 and instr(busitype,'" + busitype + "')>0";
            if (busitype == "空运进口")
            {
                //if ((Int32)redisClient.Exists("common_data_sbfs:kj") == 1)
                //{
                //    json_sbfs = redisClient.Get<string>("common_data_sbfs:kj");
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:kj", json_sbfs);
                //}
            }
            if (busitype == "空运出口")
            {
                //if (db.KeyExists("common_data_sbfs:kc"))
                //{
                //    json_sbfs = db.StringGet("common_data_sbfs:kc");
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:kc", json_sbfs);
                // }
            }
            if (busitype == "陆运进口")
            {
                //if (db.KeyExists("common_data_sbfs:lj"))
                //{
                //    json_sbfs = db.StringGet("common_data_sbfs:lj");
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:lj", json_sbfs);
                //}
            }
            if (busitype == "陆运出口")
            {
                //if (db.KeyExists("common_data_sbfs:lc"))
                //{
                //    json_sbfs = db.StringGet("common_data_sbfs:lc");
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:lc", json_sbfs);
                //}
            }
            if (busitype == "海运进口")
            {
                //if (db.KeyExists("common_data_sbfs:hj"))
                //{
                //    json_sbfs = db.StringGet("common_data_sbfs:hj");
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:hj", json_sbfs);
                // }
            }
            if (busitype == "海运出口")
            {
                if (db.KeyExists("common_data_sbfs:hc"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:hc");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:hc", json_sbfs);
                }
            }
            if (busitype == "特殊区域")
            {
                //if (db.KeyExists("common_data_sbfs:ts"))
                //{
                //    json_sbfs = db.StringGet("common_data_sbfs:ts").ToString();
                //} 
                //else
                //{
                json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data_sbfs:ts", json_sbfs);
                //}
            }
            if (busitype == "国内")
            {
                if (db.KeyExists("common_data_sbfs:gn"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:gn");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:gn", json_sbfs);
                }
            }

            string json_sbgq = "[]";//申报关区 进口口岸 
            if (db.KeyExists("common_data:sbgq"))
            {
                json_sbgq = db.StringGet("common_data:sbgq");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME from BASE_CUSTOMDISTRICT  where ENABLED=1 ORDER BY CODE";
                json_sbgq = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:sbgq", json_sbgq);
            }

            string json_jydw = "";//经营单位 
            if (db.KeyExists("jydw:" + json_user.Value<string>("CUSTOMERID")))
            {
                json_jydw = db.StringGet("jydw:" + json_user.Value<string>("CUSTOMERID"));
            }
            else
            {
                //2016-6-2 梁总提出一个改进 如果某一个经营单位 客户先 添加到自己的库了，但后来总库里面禁用了，则客户自己的库中也要禁用掉
                sql = @"SELECT T.* FROM (
                            SELECT a.CUSTOMERID, a.companychname||'('||a.companyenname||')' NAME ,a.companychname SHORTNAME, a.companyenname CODE,b.incode QUANCODE,b.name QUANNAME FROM USER_RENAME_COMPANY a 
                            left join BASE_COMPANY b 
                            on a.companyid = b.id 
                            where b.incode is not null and a.companyenname is not null and b.enabled=1) T 
                            WHERE  T.CUSTOMERID = '" + json_user.Value<string>("CUSTOMERID") + "'";
                json_jydw = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("jydw:" + json_user.Value<string>("CUSTOMERID"), json_jydw);
            }
            string json_bgfs = "[]";//报关方式 
            if (db.KeyExists("common_data:bgfs"))
            {
                json_bgfs = db.StringGet("common_data:bgfs");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME  from SYS_DECLWAY where enabled=1 order by id asc";
                json_bgfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:bgfs", json_bgfs);
            }

            string json_bzzl = "[]";//包装种类 
            if (db.KeyExists("common_data:bzzl"))
            {
                json_bzzl = db.StringGet("common_data:bzzl");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME from base_Packing";
                json_bzzl = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:bzzl", json_bzzl);
            }

            string json_myfs = "[]";//贸易方式 
            if (db.KeyExists("common_data:myfs"))
            {
                json_myfs = db.StringGet("common_data:myfs");
            }
            else
            {
                sql = @"select ID,CODE,NAME||'('||CODE||')' NAME from BASE_DECLTRADEWAY WHERE enabled=1";
                json_myfs = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:myfs", json_myfs);
            }

            string json_containertype = "[]";//集装箱类型 
            if (db.KeyExists("common_data:containertype"))
            {
                json_containertype = db.StringGet("common_data:containertype");
            }
            else
            {
                sql = "select CODE,NAME||'('||CONTAINERCODE||')' as MERGENAME,CONTAINERCODE from BASE_CONTAINERTYPE where enabled=1";
                json_containertype = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:containertype", json_containertype);
            }

            string json_containersize = "[]";//集装箱尺寸 
            if (db.KeyExists("common_data:containersize"))
            {
                json_containersize = db.StringGet("common_data:containersize");
            }
            else
            {
                sql = "select CODE,NAME as CONTAINERSIZE,NAME||'('||DECLSIZE||')' as MERGENAME,DECLSIZE from BASE_CONTAINERSIZE where enabled=1";
                json_containersize = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:containersize", json_containersize);
            }

            string json_truckno = "[]";//报关车号 
            if (db.KeyExists("common_data:truckno"))
            {
                json_truckno = db.StringGet("common_data:truckno");
            }
            else
            {
                sql = @"select t.license, t.license||'('||t.whitecard||')' as MERGENAME,t.whitecard,t1.NAME||'('|| t1.CODE||')' as UNITNO from sys_declarationcar t
                left join base_motorcade t1 on t.motorcade=t1.code where t.enabled=1";
                json_truckno = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:truckno", json_truckno);
            }

            string json_relacontainer = "[]";//关联集装箱信息  通过选择集装箱类型的CODE和集装箱尺寸的CODE,会自动匹配关联集装箱信息
            if (db.KeyExists("common_data:relacontainer"))
            {
                json_relacontainer = db.StringGet("common_data:relacontainer");
            }
            else
            {
                sql = @"select CONTAINERSIZE,CONTAINERTYPE,FORMATNAME,CONTAINERHS from rela_container t where t.enabled=1";
                json_relacontainer = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:relacontainer", json_relacontainer);
            }
            //木质包装
            string json_mzbz = "[]";
            if (db.KeyExists("common_data:mzbz"))
            {
                json_mzbz = db.StringGet("common_data:mzbz");
            }
            else
            {
                sql = @"select CODE,NAME||'('||CODE||')' NAME from SYS_WOODPACKING";
                json_mzbz = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("common_data:mzbz", json_mzbz);
            }
            //            string json_cur_usr = "";//当前用户信息
            //            sql = @"SELECT a.*,b.INTERFACECODE FROM sys_user a  left join  sys_customer b on a.customerid=b.id 
            //                  WHERE   a.ID = '" + json_user.Value<string>("ID") + "'";
            //            json_cur_usr = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            return "{jydw:" + json_jydw + ",sbfs:" + json_sbfs + ",sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",bzzl:" + json_bzzl + ",myfs:" + json_myfs + ",containertype:" + json_containertype + ",containersize:" + json_containersize + ",truckno:" + json_truckno + ",relacontainer:" + json_relacontainer + ",mzbz:" + json_mzbz + "}";
        }

        //查询条件默认值 by heguiqin 2016-08-26
        public string loadquerysetting()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            //首先判断该用户先前有没有设置查询条件
            string sql = "select * from CONFIG_QUERYSETTING where UserId='" + json_user.Value<string>("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string json = (JsonConvert.SerializeObject(DBMgr.GetDataTable(sql))).TrimStart('[').TrimEnd(']');
                return "{success:true,data:" + json + "}";
            }
            else
            {
                return "{rows:''}";
            }
        }

        //分页 by heguiqin 2016-08-26
        private string GetPageSql(string tempsql, string order, string asc)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);
            string pageSql = @"SELECT * FROM ( SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt WHERE ROWNUM <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }

        //删除报关单及其对应文件信息 by heguiqin 2016-08-26
        public string Delete()
        {
            string result = "";
            result = Extension.deleteorder(Request["ordercode"] + "");
            return result;
        }

        //报关申报单位开创加载资料
        public string selectbgsbdw()
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["Name"]))
            {
                where += " and Name like '%" + Request["Name"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["Code"]))
            {
                where += " and Code like '%" + Request["Code"] + "%'";
            }
            string sql = "SELECT * FROM BASE_COMPANY WHERE  CODE IS NOT NULL AND ENABLED=1  " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATEDATE", "desc"));
            string json = JsonConvert.SerializeObject(dt); ;
            return "{total:" + totalProperty + ",rows:" + json + "}";
        }

        //报检申报单位开窗加载资料
        public string selectbjsbdw()
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["Name"]))
            {
                where += " and Name like '%" + Request["Name"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["INSPCODE"]))
            {
                where += " and INSPCODE like '%" + Request["INSPCODE"] + "%'";
            }
            string sql = "SELECT * FROM BASE_COMPANY WHERE  INSPCODE IS NOT NULL AND ENABLED=1  " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATEDATE", "desc"));
            string json = JsonConvert.SerializeObject(dt); ;
            return "{total:" + totalProperty + ",rows:" + json + "}";
        }

        //经营单位选择窗体弹出时加载基础库数据
        public JsonResult LoadJydw()
        {
            string sql = "";
            DataTable dt;
            sql = "SELECT ID,INCODE CODE,NAME FROM BASE_COMPANY WHERE ENABLED=1 AND (INCODE LIKE '%{0}%' OR NAME LIKE '%{0}%')";
            sql = string.Format(sql, (Request["NAME"] + "").ToUpper());
            dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATEDATE", "desc"));
            var data1 = (from B in dt.AsEnumerable()
                         select new
                         {
                             ID = B["ID"],
                             CODE = B["CODE"],
                             NAME = B["NAME"],
                         }).AsQueryable();
            var json5 = new { total = totalProperty, rows = data1 };
            return Json(json5, JsonRequestBehavior.AllowGet);
        }

        //经营单位开窗后，选择资料后 更新用户库
        public string UpdateRenameCompany()
        {
            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(AppUtil.RedisIp);
            //IDatabase db = redis.GetDatabase();
            IDatabase db = SeRedis.redis.GetDatabase();
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string IDS = Request["IDS"]; //更新经营单位简称表 
            string CODES = Request["CODES"];
            string NAMES = Request["NAMES"];
            string sql = @"select * from user_rename_company where CUSTOMERID ='" + json_user.Value<string>("CUSTOMERID") + "' and companyid = '" + IDS + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string res_json = "";
            if (dt.Rows.Count == 0)
            {
                sql = @"INSERT INTO USER_RENAME_COMPANY (ID,CUSTOMERID,COMPANYID,COMPANYCHNAME,COMPANYENNAME,CREATEDATE) 
                            VALUES (USER_RENAME_COMPANY_id.Nextval,'{0}','{1}','{2}','{3}',sysdate)";
                sql = string.Format(sql, json_user.Value<string>("CUSTOMERID"), IDS, NAMES, CODES);
                DBMgr.ExecuteNonQuery(sql);

                //更新redis,同步最新经营单位简称 这里记得需要和Ini_Base_Data的数据结构保持一致 update by panhuaguo 2016-07-29         
                sql = @"SELECT T.* FROM (
                      SELECT a.CUSTOMERID, a.companychname||'('||a.companyenname||')' NAME ,a.companychname SHORTNAME, a.companyenname CODE,b.incode QUANCODE,b.name QUANNAME FROM USER_RENAME_COMPANY a 
                      left join BASE_COMPANY b on a.companyid = b.id  
                      where b.incode is not null and a.companyenname is not null and b.enabled=1) T 
                      WHERE  T.CUSTOMERID = '" + json_user.Value<string>("CUSTOMERID") + "'";
                var json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
                db.StringSet("jydw:" + json_user.Value<string>("CUSTOMERID"), json);
                res_json = "{CODE:'" + CODES + "',NAME:'" + NAMES + "',SHORTNAME:'" + NAMES + "',QUANCODE:'" + CODES + "',QUANNAME:'" + NAMES + "',data:" + json + "}";
            }
            else//如果简称库已经存在
            {
                string json = db.StringGet("jydw:" + json_user.Value<string>("CUSTOMERID"));
                res_json = "{CODE:'" + dt.Rows[0]["COMPANYENNAME"] + "',NAME:'" + dt.Rows[0]["COMPANYCHNAME"] + "',SHORTNAME:'" + dt.Rows[0]["COMPANYCHNAME"] + "',QUANCODE:'" + CODES + "',QUANNAME:'" + NAMES + "',data:" + json + "}";
            }
            return res_json;
        }

        //贸易方式开窗加载资料
        public string LoadMyfs()
        {
            string sql = @"select ID,CODE,NAME from BASE_DECLTRADEWAY WHERE CODE LIKE '%{0}%' OR NAME LIKE '%{0}%'";
            sql = string.Format(sql, Request["NAME"] + "");
            var json = JsonConvert.SerializeObject(DBMgr.GetDataTable(GetPageSql(sql, "CODE", "asc")));
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        //文件上传到web服务器
        public ActionResult UploadFile(int? chunk, string name)
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

        //非国内订单订单编辑页在首次加载或者修改时封装的方法 by panhuaguo 20160729   国内订单要复杂很多，单独放在其自身控制器里面
        public string loadorder()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ordercode = Request["ordercode"];
            string copyordercode = Request["copyordercode"];
            DataTable dt;
            string bgsb_unit = "";
            string bjsb_unit = "";
            string result = "{}";
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
                if (string.IsNullOrEmpty(copyordercode))//如果不是复制新增
                {
                    string formdata = "{STATUS:1,REPUNITNAME:'" + bgsb_unit + "',REPUNITCODE:'" + json_user.Value<string>("CUSTOMERHSCODE") + "',INSPUNITNAME:'" + bjsb_unit + "',INSPUNITCODE:'" + json_user.Value<string>("CUSTOMERCIQCODE") + "'}";
                    result = "{formdata:" + formdata + ",filedata:[]}";
                }
                else//如果是复制新增
                {
                    sql = @"select t.*,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + copyordercode + "' and rownum=1";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0]["CODE"] = DBNull.Value; dt.Rows[0]["STATUS"] = "1";
                        dt.Rows[0]["CREATEUSERID"] = DBNull.Value; dt.Rows[0]["CREATEUSERNAME"] = DBNull.Value;
                        dt.Rows[0]["SUBMITTIME"] = DBNull.Value; dt.Rows[0]["CREATETIME"] = DBNull.Value;
                        dt.Rows[0]["SUBMITUSERNAME"] = DBNull.Value; dt.Rows[0]["SUBMITUSERID"] = DBNull.Value;
                        dt.Rows[0]["SUBMITUSERPHONE"] = DBNull.Value; dt.Rows[0]["CSNAME"] = DBNull.Value;//平台客户字段需要清空
                        dt.Rows[0]["CONTAINERNO"] = DBNull.Value; dt.Rows[0]["DECLCARNO"] = DBNull.Value;
                        //报关、报检申报单位
                        dt.Rows[0]["REPUNITNAME"] = bgsb_unit; dt.Rows[0]["REPUNITCODE"] = json_user.Value<string>("CUSTOMERHSCODE");
                        dt.Rows[0]["INSPUNITNAME"] = bjsb_unit; dt.Rows[0]["INSPUNITCODE"] = json_user.Value<string>("CUSTOMERCIQCODE");
                        //件数和重量也要清空
                        dt.Rows[0]["GOODSNUM"] = DBNull.Value; dt.Rows[0]["PACKKIND"] = DBNull.Value;
                        dt.Rows[0]["GOODSGW"] = DBNull.Value; dt.Rows[0]["GOODSNW"] = DBNull.Value;
                        string formdata = JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']');
                        result = "{formdata:" + formdata + ",filedata:[]}";
                    }
                }
            }
            else //如果订单号不为空
            {
                //订单基本信息 CONTAINERTRUCK 这个字段本身不属于list_order表,虚拟出来存储集装箱和报关车号记录,是个数组形式的字符串
                string sql = @"select t.*,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + Request["ordercode"] + "' and rownum=1";
                dt = DBMgr.GetDataTable(sql);
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                sql = "select * from list_predeclcontainer t where t.ordercode='" + dt.Rows[0]["CODE"] + "' order by containerorder";
                DataTable dt_container = DBMgr.GetDataTable(sql);
                dt.Rows[0]["CONTAINERTRUCK"] = JsonConvert.SerializeObject(dt_container);
                string formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                //订单随附文件
                sql = @"select * from LIST_ATTACHMENT where instr(ordercode,'{0}') >0 
                      and ((filetype=44 or filetype=58) or ( filetype=57 AND confirmstatus = 1 )) and (abolishstatus is null or abolishstatus=0)";
                sql = string.Format(sql, ordercode);
                dt = DBMgr.GetDataTable(sql);
                string filedata = JsonConvert.SerializeObject(dt, iso);
                result = "{formdata:" + formdata + ",filedata:" + filedata + "}";
            }
            return result;
        }

        //撤单  非国内订单提交后执行撤单操作统一执行此方法 by panhuaguo 2016-08-23
        public string CancelSubmit()
        {
            string ordercode = Request["ordercode"];
            //1先判断订单的状态有没有发生变化，比如是否受理
            string sql = "select * from list_order where code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string result = "";
            if (dt.Rows[0]["STATUS"] + "" != "15")
            {
                result = "{success:false}";
            }
            else
            {
                sql = "delete from list_cspond where ordercode='" + ordercode + "'";//删除订单预配信息
                DBMgr.ExecuteNonQuery(sql);
                sql = "delete from list_times where code='" + ordercode + "' and status = '15'";//删除订单状态变更日志信息
                DBMgr.ExecuteNonQuery(sql);
                sql = "update list_order set STATUS = '10' ,SUBMITUSERNAME='',SUBMITTIME='',SUBMITUSERPHONE='' where code='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql);
                result = "{success:true}";
            }
            return result;
        }

        /*报关单管理 列表页展示*/
        public string LoadDeclarationList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            string role = Request["role"];
            string busitypeid = Request["busitypeid"];
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and ort.BUSISHORTCODE='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CUSNO"://客户编号
                        where += " and instr(ort.CUSNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "BLNO"://提运单号
                        where += " and instr(prt.BLNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and instr(prt.TRANSNAME,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(det.DECLARATIONCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(det.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件3是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "DYBZ"://打印标志
                        where += " and det.ISPRINT='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME"://订单委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and ort.SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and ort.SUBMITTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "REPTIME"://申报时间
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and det.REPTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and det.REPTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }
            if (role == "supplier") //如果是现场服务角色
            {
                where += @" and ort.SCENEDECLAREID ='" + json_user.Value<string>("CustomerId") + "' ";
            }//如果是企业服务
            if (role == "enterprise")
            {
                where += @" and ort.BUSIUNITCODE IN 
                    (SELECT b.incode QUANCODE FROM USER_RENAME_COMPANY a left join BASE_COMPANY b on a.companyid = b.id 
                    where b.incode is not null and a.companyenname is not null and a.customerid = " + json_user.Value<string>("CustomerId") + ") ";
            }
            if (role == "customer")//如果角色是客户
            {
                where += @" and ort.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";
            }           
           
            //2016-6-24 更新报关单列表显示逻辑 根据报关单对应的订单【DECLPDF】即报关单是否已关联好PDF文件，作为显示的条件 国内业务不需要去判断关联订单，因为打这两个标志的时候已经判断了           
            //DECL_TRANSNAME 预制报关单的运输工具名称
            //运输工具名称的显示需要更改为一下逻辑：根据草单中的申报库别 如果是13或者17 运输工具名称取预制报关单里面的。否则取草单的运输工具名称
            string sql = @"select det.ID,det.PREDECLCODE,det.DECLARATIONCODE,det.CODE,ort.CUSTOMERNAME ,det.REPFINISHTIME, det.CUSTOMSSTATUS ,   
                         det.ISPRINT,det.CONTRACTNO,det.GOODSNUM,det.GOODSNW,det.SHEETNUM,det.ORDERCODE,det.STARTTIME CREATEDATE,
                         det.BUSITYPE BUSITYPE,det.TRANSNAME DECL_TRANSNAME,
                         prt.TRANSNAME,prt.BUSIUNITCODE, prt.PORTCODE, prt.BLNO, prt.DECLTYPE, 
                         ort.REPWAYID ,ort.REPWAYID REPWAYNAME,ort.DECLWAY ,ort.DECLWAY DECLWAYNAME,ort.TRADEWAYCODES ,
                         ort.CUSNO ,ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.BUSISHORTNAME                                                                          
                         from list_declaration det 
                         left join list_predeclaration prt  on det.predeclcode = prt.predeclcode 
                         left join list_order ort on prt.ordercode = ort.code 
                         where (ort.DECLPDF =1 or ort.PREPDF=1) and det.isinvalid=1 and instr('" + busitypeid + "',det.busitype)>0 " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATEDATE", "desc"));
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        /*通过查询条件获取具体的报关单海关状态数据 list_receiptstatus 这个表是预制报关单海关状态回执表
         目前有两个地方在抓取 我显示时按照时间进行升序，如果有重复的状态名称,取时间早的那个时间值
         by panhuaguo 2016-6-3*/
        public string LoadCustomsReceipt()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select * from (select l.DECLSTATUS,min(l.TIMES) TIMES from list_receiptstatus l where l.DECLSTATUS is not null and 
                         l.code = '" + Request["bgdcode"] + "' group by l.DECLSTATUS) a order by a.times asc ";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + "}";
        }

    }
}
