namespace FribergBookRentals.Data
{
    public class RedirectToActionData
    {
        #region Constructors

        public RedirectToActionData()
        {
        }

        public RedirectToActionData(string action, string controller, RouteValueDictionary? routeValues = null, string? urlFragment = null)
        {
            Action = action;
            Controller = controller;
            RouteValues = routeValues;
			UrlFragment = urlFragment;
		}

        #endregion

        #region Properties

        public string Action { get; set; } = "";

        public string Controller { get; set; } = "";

        public RouteValueDictionary? RouteValues { get; set; }

		/// <summary>
		/// An optional URL fragment for targeting regions within a page (#ID).
		/// </summary>
		public string? UrlFragment { get; set; }

		#endregion
	}
}
