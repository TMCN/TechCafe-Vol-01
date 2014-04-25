using Microsoft.WindowsAzure.MobileServices;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace leapcloud
{
    namespace Model
    {
        public class TodoItem
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public int Hand { get; set; }
            public int Result { get; set; }
        }

        public class UserItem
        {
            public string id { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
        }

        public class AzureModel : INotifyPropertyChanged
        {
            private static MobileServiceClient MobileServiceClient = new MobileServiceClient("https://leapcloud.azure-mobile.net");
            private MobileServiceCollection<TodoItem, TodoItem> Items;
            private IMobileServiceTable<TodoItem> TodoTable = MobileServiceClient.GetTable<TodoItem>();
            private IMobileServiceTable<UserItem> UserTable = MobileServiceClient.GetTable<UserItem>();

            private string _UserId = "";
            public string UserId
            {
                get { return this._UserId; }
                set
                {
                    this._UserId = value;
                    OnPropertyChanged();
                }
            }

            //'-1:受付中、0:対戦中、1:勝利、2:敗北
            private int _Result = -1;
            public int Result
            {
                get { return this._Result; }
                set
                {
                    this._Result = value;
                    OnPropertyChanged();
                }
            }

            private string _ErrorString = "";
            public string ErrorString
            {
                get { return this._ErrorString; }
                set
                {
                    this._ErrorString = value;
                    OnPropertyChanged();
                }
            }

            public async Task SetUserId(string userName)
            {
                try
                {
                    await GetUserID(userName);
                    if (this.UserId.Length == 0)
                    {
                        await UserTable.InsertAsync(new UserItem {UserName = userName});
                        await GetUserID(userName);
                    }
                }
                catch (Exception ex)
                {
                    this.ErrorString = ex.Message;
                }
            }

            public async Task GetUserID(string userName)
            {
                var query = UserTable.CreateQuery();
                query.Parameters.Add("UserName",userName);

                var response = await query.ToListAsync();
                if (response.Count > 0)
                {
                    this.UserId = response[0].UserId;
                }
                else
                {
                    this.UserId = "";
                }
            }

            public async Task SetItem(int hand)
            {
                try
                {
                    await TodoTable.InsertAsync(new TodoItem {UserId = this.UserId, Hand = hand});
                    await GetItem(hand);
                }
                catch(Exception ex)
                {
                    this.ErrorString = ex.Message;
                }

            }

            public async Task GetItem(int hand)
            {
                var query = TodoTable.CreateQuery();
                query.Parameters.Add("UserId", UserId);
                var response = await query.ToListAsync();
                if (response.Count > 0)
                {
                    this.Result = response[0].Result;
                }
                else
                {
                    this.Result = -1;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }        
    }
}
