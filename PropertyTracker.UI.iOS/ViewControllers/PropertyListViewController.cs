﻿
using System;
using System.Drawing;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PropertyTracker.Core.ViewModels;
using PropertyTracker.UI.iOS.Common;
using PropertyTracker.UI.iOS.Views;

// Make sure namespace is same in designer.cs - Xamarin skips adding subfolders to namespace!
using PropertyTracker.Dto.Models;


namespace PropertyTracker.UI.iOS.ViewControllers
{
	public partial class PropertyListViewController : MvxTableViewController
	{
        private const string PropertyCellId = "PropertyCell";
		private PropertyListOptionsViewController _optionsVC = null;

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public PropertyListViewController(IntPtr handle)
			: base(handle)
		{
		}

		public new PropertyListViewModel ViewModel
		{
			get { return (PropertyListViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public override void ViewDidLoad()
		{            
			base.ViewDidLoad();
			//NavigationController.NavigationBarHidden = false;

			var logoutButton = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Bordered, null);
			NavigationItem.LeftBarButtonItem = logoutButton;

			var addPropertyButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, (o, e) => {
				var controller = this.CreateViewControllerFor<AddPropertyViewModel> () as AddPropertyViewController;
				NavigationController.PushViewController (controller, true);
			});

			NavigationItem.RightBarButtonItem = addPropertyButton;

			this.SetTitleAndTabBarItem(ViewModel.TabTitle, ViewModel.TabImageName, ViewModel.TabSelectedImageName, ViewModel.TabBadgeValue);

            //var source = new MvxStandardTableViewSource(TableView, UITableViewCellStyle.Subtitle, new NSString(PropertyCellId), "TitleText Name;DetailText City",
            //    UITableViewCellAccessory.DisclosureIndicator);

			var source = new MvxStandardTableViewSource (TableView, PropertyListCell.Key);

			//var source = new MvxSimpleTableViewSource(TableView,

			TableView.Source = source;
          
			//var inset = TableView.ContentInset;
			//inset.Top -= 176;
			//TableView.ContentInset = inset;
		
			var set = this.CreateBindingSet<PropertyListViewController, PropertyListViewModel>();
			set.Bind(source).To(vm => vm.Properties);
            
			set.Bind (_optionsVC.SearchBar).For(v => v.Text).To (vm => vm.NameFilter);
			set.Bind (_optionsVC.CityFilterLabel).To (vm => vm.CityFilter);
			set.Bind (_optionsVC.StateProvFilterLabel).To (vm => vm.StateFilter);
			set.Bind (_optionsVC.SortOrderSwitch).To (vm => vm.SortAscending);


			set.Bind(logoutButton).To(vm => vm.LogoutCommand);
			set.Bind(TabBarItem).For(v => v.Title).To(vm => vm.TabTitle);
			set.Bind(TabBarItem).For(v => v.BadgeValue).To(vm => vm.TabBadgeValue);
			set.Bind (GetPropertiesButtonItem).To (vm => vm.GetPropertiesCommand);
			set.Bind (GetMorePropertiesButtonItem).To (vm => vm.GetMorePropertiesCommand);

			//set.Bind(Title).To(vm => vm.TabTitle);
			//set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.TabTitle);

			set.Apply();

            // Reload clicked - run command and hide search options
            /*
		    GetPropertiesButtonItem.Clicked += (sender, args) =>
		    {
		        //HideSearchOptions(true);
                //TableView.ReloadData();
		    };
            */

            // Because Options VC is not an MVVM-based view controller, we have to handle the event handling manually.
            // #future make this options VC backed by a MVVM ViewModel, so we don't have to do this crap here.            

			_optionsVC.SearchBar.SearchButtonClicked += (object sender, EventArgs e) => _optionsVC.SearchBar.ResignFirstResponder ();
				
			_optionsVC.SearchBar.CancelButtonClicked += (object sender, EventArgs e) => {
				// Bound variable - this will clear search bar
				ViewModel.NameFilter = "";
				_optionsVC.SearchBar.ResignFirstResponder ();
			    HideSearchOptions(true);
			};

            _optionsVC.CityFilterTapGestureRecognizer.AddTarget(() =>
            {
				var controller = this.CreateViewControllerFor<CityPickerViewModel>(new 
					{
						city = _optionsVC.CityFilterLabel.Text, 
					 	requestedViewId = ViewModel.ViewInstanceId
					}) as CityPickerViewController;

                NavigationController.PushViewController(controller, true);				
            });

            _optionsVC.StateFilterTapGestureRecognizer.AddTarget(() =>
            {
                var controller = this.CreateViewControllerFor<StatePickerViewModel>(new
                {
                    state = _optionsVC.StateProvFilterLabel.Text,
                    requestedViewId = ViewModel.ViewInstanceId
                }) as StatePickerViewController;
                NavigationController.PushViewController(controller, true);
            });

			_optionsVC.SortColumnSegmentControl.ValueChanged += (object sender, EventArgs e) => {
				var segment = sender as UISegmentedControl;
				switch((PropertyListOptionsViewController.SortColumn) segment.SelectedSegment)
				{
				case PropertyListOptionsViewController.SortColumn.Name:
					ViewModel.SortColumn = PropertyListRequest.NameColumn;
					break;

				case PropertyListOptionsViewController.SortColumn.City:
					ViewModel.SortColumn = PropertyListRequest.CityColumn;
					break;

				case PropertyListOptionsViewController.SortColumn.State:
					ViewModel.SortColumn = PropertyListRequest.StateColumn;
					break;
				}
			};

			//NavigationItem.Title;

            // Data is fetched after
            TableView.ReloadData();

		    //ViewModel.Properties.CollectionChanged += (sender, args) => HideSearchOptions(true);
		}

	    private bool _searchOptionsHidden;
	    private void HideSearchOptions(bool force = false)
	    {
	        if (_searchOptionsHidden && !force) return;

	        // Only hide search options once
	        TableView.ContentOffset = new PointF(0, -64 + 176);
	        _searchOptionsHidden = true;
	    }



		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//NavigationController.Nav
			//NavigationItem.BackBarButtonItem = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Plain, (o, e) => { });
			// NavigationItem.BackBarButtonItem.Title = "Logout";

			//var navigationController = NavigationController;
			// var item = NavigationItem;

			//NavigationController.NavigationBar.PushNavigationItem(item, true);

			//item.LeftBarButtonItem = new UIBarButtonItem("Logout A", UIBarButtonItemStyle.Bordered, (o, e) => { });

			// We want to just share MainViewController's navigation bar.

			//NavigationController.NavigationBarHidden = false;
			//Console.WriteLine("Property Navigation Controller:" + NavigationController);
		
            HideSearchOptions();

			// Because we are updating a static City/State Filter cells within OptionsVC via binding property - we need
			// to reload the table view so the new cells are redrawn.
			_optionsVC.TableView.ReloadData ();

		}



		private void Logout()
		{

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);


			//ViewModel.ChangeStuff();

		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			// This is called before ViewDidLoad, and this is a container view 
            // within this view controller - so it does not have a ViewModel 
			if(segue.Identifier == "PropertyListOptionsSegue")
			{
				_optionsVC = segue.DestinationViewController as PropertyListOptionsViewController;
				//Console.WriteLine (_optionsVC.Ci);

			}
		}
	}


//	public class TableSource : MvxTableViewSource
//	{
//		public TableSource(UITableView tableView)
//			: base(tableView)
//		{
//			tableView.RegisterNibForCellReuse(PropertyCell.Nib);
//		}
//
//		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
//		{
//			return PropertyCell.GetCellHeight();
//		}
//
//		protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath,
//			object item)
//		{
//			NSString cellIdentifier;
//			if (item is Property)
//			{
//				cellIdentifier = PropertyCell.Id;
//			}
//			/*
//			else if (item is Dog)
//			{
//				cellIdentifier = DogCellIdentifier;
//			}
//			else
//			{
//				throw new ArgumentException("Unknown animal of type " + item.GetType().Name);
//			}
//			*/
//
//			return (UITableViewCell) TableView.DequeueReusableCell(cellIdentifier, indexPath);
//		}
//	}

}