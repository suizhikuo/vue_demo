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
using Maticsoft.Common;
using LTP.Accounts.Bus;
namespace Maticsoft.Web.A_City
{
    public partial class Modify : Page
    {       

        		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (Request.Params["id"] != null && Request.Params["id"].Trim() != "")
				{
					int ID=(Convert.ToInt32(Request.Params["id"]));
					ShowInfo(ID);
				}
			}
		}
			
	private void ShowInfo(int ID)
	{
		Maticsoft.BLL.A_City bll=new Maticsoft.BLL.A_City();
		Maticsoft.Model.A_City model=bll.GetModel(ID);
		this.lblID.Text=model.ID.ToString();
		this.txtPostCode.Text=model.PostCode;
		this.txtTelRegionCode.Text=model.TelRegionCode;
		this.txtTelSizeNumber.Text=model.TelSizeNumber.ToString();
		this.txtCityLevel.Text=model.CityLevel;
		this.txtCity.Text=model.City;
		this.txtRegionId.Text=model.RegionId.ToString();
		this.txtRegionLId.Text=model.RegionLId.ToString();
		this.txtGBCode.Text=model.GBCode.ToString();
		this.txtProvinceId.Text=model.ProvinceId.ToString();
		this.chkStateEnable.Checked=model.StateEnable;

	}

		public void btnSave_Click(object sender, EventArgs e)
		{
			
			string strErr="";
			if(this.txtPostCode.Text.Trim().Length==0)
			{
				strErr+="PostCode不能为空！\\n";	
			}
			if(this.txtTelRegionCode.Text.Trim().Length==0)
			{
				strErr+="TelRegionCode不能为空！\\n";	
			}
			if(!PageValidate.IsNumber(txtTelSizeNumber.Text))
			{
				strErr+="TelSizeNumber格式错误！\\n";	
			}
			if(this.txtCityLevel.Text.Trim().Length==0)
			{
				strErr+="CityLevel不能为空！\\n";	
			}
			if(this.txtCity.Text.Trim().Length==0)
			{
				strErr+="City不能为空！\\n";	
			}
			if(!PageValidate.IsNumber(txtRegionId.Text))
			{
				strErr+="RegionId格式错误！\\n";	
			}
			if(!PageValidate.IsNumber(txtRegionLId.Text))
			{
				strErr+="RegionLId格式错误！\\n";	
			}
			if(!PageValidate.IsNumber(txtGBCode.Text))
			{
				strErr+="GBCode格式错误！\\n";	
			}
			if(!PageValidate.IsNumber(txtProvinceId.Text))
			{
				strErr+="ProvinceId格式错误！\\n";	
			}

			if(strErr!="")
			{
				MessageBox.Show(this,strErr);
				return;
			}
			int ID=int.Parse(this.lblID.Text);
			string PostCode=this.txtPostCode.Text;
			string TelRegionCode=this.txtTelRegionCode.Text;
			int TelSizeNumber=int.Parse(this.txtTelSizeNumber.Text);
			string CityLevel=this.txtCityLevel.Text;
			string City=this.txtCity.Text;
			int RegionId=int.Parse(this.txtRegionId.Text);
			int RegionLId=int.Parse(this.txtRegionLId.Text);
			int GBCode=int.Parse(this.txtGBCode.Text);
			int ProvinceId=int.Parse(this.txtProvinceId.Text);
			bool StateEnable=this.chkStateEnable.Checked;


			Maticsoft.Model.A_City model=new Maticsoft.Model.A_City();
			model.ID=ID;
			model.PostCode=PostCode;
			model.TelRegionCode=TelRegionCode;
			model.TelSizeNumber=TelSizeNumber;
			model.CityLevel=CityLevel;
			model.City=City;
			model.RegionId=RegionId;
			model.RegionLId=RegionLId;
			model.GBCode=GBCode;
			model.ProvinceId=ProvinceId;
			model.StateEnable=StateEnable;

			Maticsoft.BLL.A_City bll=new Maticsoft.BLL.A_City();
			bll.Update(model);
			Maticsoft.Common.MessageBox.ShowAndRedirect(this,"保存成功！","list.aspx");

		}


        public void btnCancle_Click(object sender, EventArgs e)
        {
            Response.Redirect("list.aspx");
        }
    }
}
