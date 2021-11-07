using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
namespace Maticsoft.Web.A_City
{
    public partial class Show : Page
    {        
        		public string strid=""; 
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (Request.Params["id"] != null && Request.Params["id"].Trim() != "")
				{
					strid = Request.Params["id"];
					int ID=(Convert.ToInt32(strid));
					ShowInfo(ID);
				}
			}
		}
		
	private void ShowInfo(int ID)
	{
		Maticsoft.BLL.A_City bll=new Maticsoft.BLL.A_City();
		Maticsoft.Model.A_City model=bll.GetModel(ID);
		this.lblID.Text=model.ID.ToString();
		this.lblPostCode.Text=model.PostCode;
		this.lblTelRegionCode.Text=model.TelRegionCode;
		this.lblTelSizeNumber.Text=model.TelSizeNumber.ToString();
		this.lblCityLevel.Text=model.CityLevel;
		this.lblCity.Text=model.City;
		this.lblRegionId.Text=model.RegionId.ToString();
		this.lblRegionLId.Text=model.RegionLId.ToString();
		this.lblGBCode.Text=model.GBCode.ToString();
		this.lblProvinceId.Text=model.ProvinceId.ToString();
		this.lblStateEnable.Text=model.StateEnable?"是":"否";

	}


    }
}
