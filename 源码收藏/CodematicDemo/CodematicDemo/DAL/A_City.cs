using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Maticsoft.DBUtility;//Please add references
namespace Maticsoft.DAL
{
	/// <summary>
	/// 数据访问类:A_City
	/// </summary>
	public partial class A_City
	{
		public A_City()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperSQL.GetMaxID("ID", "A_City"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from A_City");
			strSql.Append(" where ID=@ID ");
			SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
			parameters[0].Value = ID;

			return DbHelperSQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(Maticsoft.Model.A_City model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into A_City(");
			strSql.Append("ID,PostCode,TelRegionCode,TelSizeNumber,CityLevel,City,RegionId,RegionLId,GBCode,ProvinceId,StateEnable)");
			strSql.Append(" values (");
			strSql.Append("@ID,@PostCode,@TelRegionCode,@TelSizeNumber,@CityLevel,@City,@RegionId,@RegionLId,@GBCode,@ProvinceId,@StateEnable)");
			SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@PostCode", SqlDbType.VarChar,6),
					new SqlParameter("@TelRegionCode", SqlDbType.VarChar,10),
					new SqlParameter("@TelSizeNumber", SqlDbType.Int,4),
					new SqlParameter("@CityLevel", SqlDbType.VarChar,20),
					new SqlParameter("@City", SqlDbType.VarChar,50),
					new SqlParameter("@RegionId", SqlDbType.Int,4),
					new SqlParameter("@RegionLId", SqlDbType.Int,4),
					new SqlParameter("@GBCode", SqlDbType.Int,4),
					new SqlParameter("@ProvinceId", SqlDbType.Int,4),
					new SqlParameter("@StateEnable", SqlDbType.Bit,1)};
			parameters[0].Value = model.ID;
			parameters[1].Value = model.PostCode;
			parameters[2].Value = model.TelRegionCode;
			parameters[3].Value = model.TelSizeNumber;
			parameters[4].Value = model.CityLevel;
			parameters[5].Value = model.City;
			parameters[6].Value = model.RegionId;
			parameters[7].Value = model.RegionLId;
			parameters[8].Value = model.GBCode;
			parameters[9].Value = model.ProvinceId;
			parameters[10].Value = model.StateEnable;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Maticsoft.Model.A_City model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update A_City set ");
			strSql.Append("PostCode=@PostCode,");
			strSql.Append("TelRegionCode=@TelRegionCode,");
			strSql.Append("TelSizeNumber=@TelSizeNumber,");
			strSql.Append("CityLevel=@CityLevel,");
			strSql.Append("City=@City,");
			strSql.Append("RegionId=@RegionId,");
			strSql.Append("RegionLId=@RegionLId,");
			strSql.Append("GBCode=@GBCode,");
			strSql.Append("ProvinceId=@ProvinceId,");
			strSql.Append("StateEnable=@StateEnable");
			strSql.Append(" where ID=@ID ");
			SqlParameter[] parameters = {
					new SqlParameter("@PostCode", SqlDbType.VarChar,6),
					new SqlParameter("@TelRegionCode", SqlDbType.VarChar,10),
					new SqlParameter("@TelSizeNumber", SqlDbType.Int,4),
					new SqlParameter("@CityLevel", SqlDbType.VarChar,20),
					new SqlParameter("@City", SqlDbType.VarChar,50),
					new SqlParameter("@RegionId", SqlDbType.Int,4),
					new SqlParameter("@RegionLId", SqlDbType.Int,4),
					new SqlParameter("@GBCode", SqlDbType.Int,4),
					new SqlParameter("@ProvinceId", SqlDbType.Int,4),
					new SqlParameter("@StateEnable", SqlDbType.Bit,1),
					new SqlParameter("@ID", SqlDbType.Int,4)};
			parameters[0].Value = model.PostCode;
			parameters[1].Value = model.TelRegionCode;
			parameters[2].Value = model.TelSizeNumber;
			parameters[3].Value = model.CityLevel;
			parameters[4].Value = model.City;
			parameters[5].Value = model.RegionId;
			parameters[6].Value = model.RegionLId;
			parameters[7].Value = model.GBCode;
			parameters[8].Value = model.ProvinceId;
			parameters[9].Value = model.StateEnable;
			parameters[10].Value = model.ID;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from A_City ");
			strSql.Append(" where ID=@ID ");
			SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
			parameters[0].Value = ID;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from A_City ");
			strSql.Append(" where ID in ("+IDlist + ")  ");
			int rows=DbHelperSQL.ExecuteSql(strSql.ToString());
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Maticsoft.Model.A_City GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 ID,PostCode,TelRegionCode,TelSizeNumber,CityLevel,City,RegionId,RegionLId,GBCode,ProvinceId,StateEnable from A_City ");
			strSql.Append(" where ID=@ID ");
			SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
			parameters[0].Value = ID;

			Maticsoft.Model.A_City model=new Maticsoft.Model.A_City();
			DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Maticsoft.Model.A_City DataRowToModel(DataRow row)
		{
			Maticsoft.Model.A_City model=new Maticsoft.Model.A_City();
			if (row != null)
			{
				if(row["ID"]!=null && row["ID"].ToString()!="")
				{
					model.ID=int.Parse(row["ID"].ToString());
				}
				if(row["PostCode"]!=null)
				{
					model.PostCode=row["PostCode"].ToString();
				}
				if(row["TelRegionCode"]!=null)
				{
					model.TelRegionCode=row["TelRegionCode"].ToString();
				}
				if(row["TelSizeNumber"]!=null && row["TelSizeNumber"].ToString()!="")
				{
					model.TelSizeNumber=int.Parse(row["TelSizeNumber"].ToString());
				}
				if(row["CityLevel"]!=null)
				{
					model.CityLevel=row["CityLevel"].ToString();
				}
				if(row["City"]!=null)
				{
					model.City=row["City"].ToString();
				}
				if(row["RegionId"]!=null && row["RegionId"].ToString()!="")
				{
					model.RegionId=int.Parse(row["RegionId"].ToString());
				}
				if(row["RegionLId"]!=null && row["RegionLId"].ToString()!="")
				{
					model.RegionLId=int.Parse(row["RegionLId"].ToString());
				}
				if(row["GBCode"]!=null && row["GBCode"].ToString()!="")
				{
					model.GBCode=int.Parse(row["GBCode"].ToString());
				}
				if(row["ProvinceId"]!=null && row["ProvinceId"].ToString()!="")
				{
					model.ProvinceId=int.Parse(row["ProvinceId"].ToString());
				}
				if(row["StateEnable"]!=null && row["StateEnable"].ToString()!="")
				{
					if((row["StateEnable"].ToString()=="1")||(row["StateEnable"].ToString().ToLower()=="true"))
					{
						model.StateEnable=true;
					}
					else
					{
						model.StateEnable=false;
					}
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,PostCode,TelRegionCode,TelSizeNumber,CityLevel,City,RegionId,RegionLId,GBCode,ProvinceId,StateEnable ");
			strSql.Append(" FROM A_City ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" ID,PostCode,TelRegionCode,TelSizeNumber,CityLevel,City,RegionId,RegionLId,GBCode,ProvinceId,StateEnable ");
			strSql.Append(" FROM A_City ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM A_City ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			object obj = DbHelperSQL.GetSingle(strSql.ToString());
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.ID desc");
			}
			strSql.Append(")AS Row, T.*  from A_City T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			SqlParameter[] parameters = {
					new SqlParameter("@tblName", SqlDbType.VarChar, 255),
					new SqlParameter("@fldName", SqlDbType.VarChar, 255),
					new SqlParameter("@PageSize", SqlDbType.Int),
					new SqlParameter("@PageIndex", SqlDbType.Int),
					new SqlParameter("@IsReCount", SqlDbType.Bit),
					new SqlParameter("@OrderType", SqlDbType.Bit),
					new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
					};
			parameters[0].Value = "A_City";
			parameters[1].Value = "ID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

