using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_User_UserWithdrawalComplete : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];
		// 未ログインのエラー表示
		if (ht == null)
		{
			Response.Redirect("~/Default.aspx");
			return;
		}
	}
	protected void btnTopPage_Click(object sender, EventArgs e)
	{
		Session.Clear();
		Response.Redirect("~/Default.aspx");
	}

	protected void tThreeSeconds_Tick(object sender, EventArgs e)
	{
		Session.Clear();
		Response.Redirect("~/Default.aspx");
	}
}