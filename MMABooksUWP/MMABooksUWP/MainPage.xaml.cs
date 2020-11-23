using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using MMABooksUWP.Models;
using MMABooksUWP.Services;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Net.Http;

namespace MMABooksUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Customer selected = null;
        private HttpDataService service;

        public Customer Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public ObservableCollection<State> States { get; private set; } = new ObservableCollection<State>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            service = new HttpDataService("http://localhost:5000/api");
            List<State> states = await service.GetAsync<List<State>>("states");
            foreach (State s in states)
                this.States.Add(s);
            ClearCustomerDetails();
            EnableFields(false);
            EnableButtons("pageLoad");
        }

        private async void findBtn_Click(object sender, RoutedEventArgs e) // finds a customer based off the customer id textbox
        {
            string customerId = this.customerIdTxt.Text;
            try 
            {
                Selected = await service.GetAsync<Customer>("customers\\" + customerId, null, true);
                DisplayCustomerDetails();
                EnableButtons("found");

            }
            catch
            {
                var messageDialog = new MessageDialog("A customer with that customer id cannot be found.");
                await messageDialog.ShowAsync();
                Selected = null;
                ClearCustomerDetails();
            }
        }

        private async void deleteBtn_Click(object sender, RoutedEventArgs e) //deletes the currently displayed customer based off customer id
        {
            if (Selected != null)
            {
                int customerId = Selected.CustomerId;
                if (await service.DeleteAsync("customers\\" + customerId))
                {
                    Selected = null;
                    this.customerIdTxt.Text = "";
                    ClearCustomerDetails();
                    EnableButtons("pageLoad");
                    var messageDialog = new MessageDialog("Customer was deleted.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    var messageDialog = new MessageDialog("There was a problem deleting that customer.");
                    await messageDialog.ShowAsync();
                }
            }
        }

        private void editBtn_Click(object sender, RoutedEventArgs e) //disables the customer id box and enables the other textboxes and the buttons for edditing
        {
            this.customerIdTxt.IsEnabled = false;
            EnableFields(true);
            EnableButtons("editing");
        }

        private void addBtn_Click(object sender, RoutedEventArgs e) //clears the textboxes and enables the correct textboxes and buttons for adding a customer.
        {
            Selected = null;
            this.customerIdTxt.Text = "";
            this.customerIdTxt.IsEnabled = false;
            ClearCustomerDetails();
            EnableFields(true);
            EnableButtons("adding");
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e) // saves the changes made when adding or edditing
        {
            // adding
            if (Selected == null)
            {
                Customer newCustomer = new Customer();
                newCustomer.Name = this.customerNameTxt.Text;
                newCustomer.Address = this.customerAddressTxt.Text;
                newCustomer.City = this.customerCityTxt.Text;
                newCustomer.ZipCode = this.customerZipcodeTxt.Text;
                State selectedState = (State)this.customerStateCBox.SelectedItem;
                newCustomer.StateCode = selectedState.StateCode;
                HttpResponseMessage response = await service.PostAsJsonAsync<Customer>("customers", newCustomer, true);
                if (response.IsSuccessStatusCode) 
                {
                    // the customer id is at the end of response.Headers.Location.AbsolutePath
                    string url = response.Headers.Location.AbsolutePath;
                    int index = url.LastIndexOf("/");
                    string customerId = url.Substring(index + 1);
                    newCustomer.CustomerId = int.Parse(customerId);
                    Selected = newCustomer;
                    this.customerIdTxt.Text = Selected.CustomerId.ToString();
                    this.customerIdTxt.IsEnabled = true;
                    DisplayCustomerDetails();
                    EnableButtons("found");
                    var messageDialog = new MessageDialog("Customer was added.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    var messageDialog = new MessageDialog("There was a problem adding that customer.");
                    await messageDialog.ShowAsync();
                }
            }
            // editing
            else
            {
                Customer updatedCustomer = new Customer();
                updatedCustomer.CustomerId = Selected.CustomerId;
                updatedCustomer.Name = this.customerNameTxt.Text;
                updatedCustomer.Address = this.customerAddressTxt.Text;
                updatedCustomer.City = this.customerCityTxt.Text;
                updatedCustomer.ZipCode = this.customerZipcodeTxt.Text;
                State selectedState = (State)this.customerStateCBox.SelectedItem;
                updatedCustomer.StateCode = selectedState.StateCode;
                if (await service.PutAsJsonAsync<Customer>("customers\\" + updatedCustomer.CustomerId, updatedCustomer))
                {
                    Selected = updatedCustomer;
                    DisplayCustomerDetails();
                    this.customerIdTxt.IsEnabled = true;
                    EnableButtons("found");
                    var messageDialog = new MessageDialog("Customer was updated.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    var messageDialog = new MessageDialog("There was a problem updating that customer.");
                    await messageDialog.ShowAsync();
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e) // runs when the cancel button is clicked. resets the app to default state
        {
            // adding
            if (Selected == null) //when adding a customer cancel resets the textboxes to empty
            {
                this.customerIdTxt.IsEnabled = true;
                ClearCustomerDetails();
                EnableFields(false);
                EnableButtons("pageLoad");
            }
            // editing 
            else //when edditing an entry, the orriginal entry is displayed.
            {
                DisplayCustomerDetails();
                this.customerIdTxt.IsEnabled = true;
                EnableButtons("found");
            }

        }

        private void DisplayCustomerDetails() // displays the customers info in the textboxes
        {
            this.customerNameTxt.Text = Selected.Name;
            this.customerAddressTxt.Text = Selected.Address;
            this.customerCityTxt.Text = Selected.City;
            this.customerZipcodeTxt.Text = Selected.ZipCode;
            int stateIndex = this.FindStateIndex(Selected.StateCode);
            this.customerStateCBox.SelectedIndex = stateIndex;
            EnableFields(false);
        }

        private void ClearCustomerDetails() // clears the textboxes
        {
            this.customerNameTxt.Text = "";
            this.customerAddressTxt.Text = "";
            this.customerCityTxt.Text = "";
            this.customerZipcodeTxt.Text = "";
            this.customerStateCBox.SelectedIndex = -1;
            EnableFields(false);
        }

        private int FindStateIndex(string stateCode) //returns the index of the state in the statecode array
        {
            int index = 0;
            foreach (State s in this.States)
            {
                if (s.StateCode == stateCode)
                    return index;
                index++;
            }
            return -1;
        }

        private void UpdateCustomerDetails() // updates the customers details to the ones written in the input
        {
            Selected.Name = this.customerNameTxt.Text;
            Selected.Address = this.customerAddressTxt.Text;
            Selected.City = this.customerCityTxt.Text;
            Selected.ZipCode = this.customerZipcodeTxt.Text;
            State selectedState = (State)this.customerStateCBox.SelectedItem;
            Selected.StateCode = selectedState.StateCode;
        }

        private void EnableFields(bool enabled = true) //allows the text feilds to be edited or not
        {
            this.customerNameTxt.IsEnabled = enabled;
            this.customerAddressTxt.IsEnabled = enabled;
            this.customerCityTxt.IsEnabled = enabled;
            this.customerZipcodeTxt.IsEnabled = enabled;
            this.customerStateCBox.IsEnabled = enabled;
        }

        private void EnableButtons(string state) //changes which buttons can be used based on the state of the program
        {
            switch (state)
            {
                case "pageLoad":
                    this.deleteBtn.IsEnabled = false;
                    this.editBtn.IsEnabled = false;
                    this.saveBtn.IsEnabled = false;
                    this.cancelBtn.IsEnabled = false;
                    this.findBtn.IsEnabled = true;
                    this.addBtn.IsEnabled = true;
                    break;
                case "editing": case "adding":
                    this.deleteBtn.IsEnabled = false;
                    this.editBtn.IsEnabled = false;
                    this.addBtn.IsEnabled = false;
                    this.findBtn.IsEnabled = false;
                    this.saveBtn.IsEnabled = true;
                    this.cancelBtn.IsEnabled = true;
                    break;
                case "found":
                    this.saveBtn.IsEnabled = false;
                    this.cancelBtn.IsEnabled = false;
                    this.deleteBtn.IsEnabled = true;
                    this.editBtn.IsEnabled = true;
                    this.addBtn.IsEnabled = true;
                    this.findBtn.IsEnabled = true;
                    break;
            }

        }
    }
}
