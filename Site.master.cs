using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Site : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		
	}

	protected void btnUserModify_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/User/UserModifyInput.aspx");
	}

	protected void btnUserWithdrawal_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/User/UserWithdrawalInput.aspx");
	}

	protected void btnLogOut_Click(object sender, EventArgs e)
	{
		Session.Clear();
		Response.Redirect("~/Default.aspx");
	}

}
