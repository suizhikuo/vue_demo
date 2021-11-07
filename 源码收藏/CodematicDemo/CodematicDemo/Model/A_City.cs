using System;
namespace Maticsoft.Model
{
	/// <summary>
	/// A_City:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class A_City
	{
		public A_City()
		{}
		#region Model
		private int _id;
		private string _postcode;
		private string _telregioncode;
		private int? _telsizenumber;
		private string _citylevel;
		private string _city;
		private int? _regionid;
		private int? _regionlid;
		private int? _gbcode;
		private int? _provinceid;
		private bool _stateenable;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PostCode
		{
			set{ _postcode=value;}
			get{return _postcode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string TelRegionCode
		{
			set{ _telregioncode=value;}
			get{return _telregioncode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? TelSizeNumber
		{
			set{ _telsizenumber=value;}
			get{return _telsizenumber;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CityLevel
		{
			set{ _citylevel=value;}
			get{return _citylevel;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string City
		{
			set{ _city=value;}
			get{return _city;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? RegionId
		{
			set{ _regionid=value;}
			get{return _regionid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? RegionLId
		{
			set{ _regionlid=value;}
			get{return _regionlid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? GBCode
		{
			set{ _gbcode=value;}
			get{return _gbcode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? ProvinceId
		{
			set{ _provinceid=value;}
			get{return _provinceid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool StateEnable
		{
			set{ _stateenable=value;}
			get{return _stateenable;}
		}
		#endregion Model

	}
}

