using Chatter.Shared.Models.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatter.Client.Pages
{
    public partial class Index : ComponentBase, IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AccessTokenProvider { get; set; }

        [Inject]
        public AccessTokenStorage storage { get; set; }
        private HubConnection hubConnection;
        private List<MessageViewModel> messages = new List<MessageViewModel>();
        private string _hubUrl;
        private IList<RoomViewModel> rooms = new List<RoomViewModel>();
        private IList<UserViewModel> users = new List<UserViewModel>();
        private InputChatModel inputModel = new InputChatModel();
        private ElementReference myref;
        private ElementReference refChatBox;
        private string username = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string baseUrl = NavigationManager.BaseUri;

                _hubUrl = baseUrl.TrimEnd('/') + "/chatHub";


                hubConnection = new HubConnectionBuilder()
                   .WithUrl(_hubUrl, options =>
                   {
                       options.AccessTokenProvider = async () =>
                       {
                           var accessTokenResult = await AccessTokenProvider.GetAuthenticationStateAsync();

                           return await storage.GetTokenAsync();
                       };
                   })
                   .Build();

                hubConnection.On<MessageViewModel>("newMessage", async (message) =>
                {
                    messages.Insert(0, message);
                    StateHasChanged();
                });
                hubConnection.On<string>("onError", async (message) =>
                {
                    MessageViewModel msg = new MessageViewModel();
                    msg.Content = message;

                    messages.Insert(0, msg);
                    StateHasChanged();
                });
                hubConnection.On<UserViewModel>("joinedRoom", async (message) =>
                {
                    inputModel.CurrentRoom = message.CurrentRoom;
                    messages.Clear();
                    IList<MessageViewModel> mess = await hubConnection.InvokeAsync<IList<MessageViewModel>>("GetMessageHistory", inputModel.Room);
                    foreach (var m in mess)
                    {
                        messages.Insert(0, m);
                    }
                    StateHasChanged();
                });


                hubConnection.On<UserViewModel>("removeUser", async (message) =>
                {
                    users = await hubConnection.InvokeAsync<IList<UserViewModel>>("GetUsers", message.CurrentRoom);
                    StateHasChanged();
                });
                hubConnection.On<UserViewModel>("addUser", async (message) =>
                {
                    users = await hubConnection.InvokeAsync<IList<UserViewModel>>("GetUsers", message.CurrentRoom);
                    StateHasChanged();
                });
                hubConnection.On<RoomViewModel>("removeChatRoom", async (message) =>
                {
                    rooms = await hubConnection.InvokeAsync<IList<RoomViewModel>>("GetRooms");
                    StateHasChanged();
                });
                hubConnection.On<RoomViewModel>("addChatRoom", async (message) =>
                {
                    rooms = await hubConnection.InvokeAsync<IList<RoomViewModel>>("GetRooms");
                    StateHasChanged();
                });
                hubConnection.On<string>("onRoomDeleted", async (message) =>
                {
                    inputModel.Room = "Lobby";
                    MessageViewModel msg = new MessageViewModel();
                    msg.Content = $"Room was deleted, you are now moved back to {inputModel.Room}";

                    messages.Insert(0, msg);
                    await JoinRoom();
                    StateHasChanged();
                });


                await hubConnection.StartAsync();
                username = await hubConnection.InvokeAsync<string>("WhoAmI");
                inputModel.Room = "Lobby";
                inputModel.Recipent = "Room";
                rooms = await hubConnection.InvokeAsync<IList<RoomViewModel>>("GetRooms");

                await JoinRoom();


            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                //  this.StateHasChanged();
            }
        }
        private async void DeleteRoom()
        {
            try
            {
                await hubConnection.InvokeAsync<IList<UserViewModel>>("DeleteRoom", inputModel.Room);
                inputModel.Room = "Lobby";
            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                this.StateHasChanged();
            }
        }
        private async void CreateRoom()
        {
            try
            {
                await hubConnection.InvokeAsync<IList<UserViewModel>>("CreateRoom", inputModel.NewRoom);
                inputModel.NewRoom = "";


            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                this.StateHasChanged();
            }
        }

        private async Task JoinRoom()
        {
            try
            {
                await hubConnection.InvokeAsync("Join", inputModel.Room);
                users = await hubConnection.InvokeAsync<IList<UserViewModel>>("GetUsers", inputModel.Room);

            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                this.StateHasChanged();
            }
        }

        private async Task Send()
        {
            try
            {

                if (inputModel.Recipent.Equals("Room"))
                {
                    await hubConnection.SendAsync("SendToRoom", inputModel.CurrentRoom, inputModel.Message); this.StateHasChanged();
                }
                else
                {
                    await hubConnection.SendAsync("SendPrivate", inputModel.Recipent, inputModel.Message); this.StateHasChanged();
                }

                inputModel.Message = "";
            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                this.StateHasChanged();
            }

        }

        public bool IsConnected()
        {
            try
            {


                return hubConnection.State == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                MessageViewModel msg = new MessageViewModel();
                msg.Content = ex.Message;
                messages.Insert(0, msg);
            }
            finally
            {
                this.StateHasChanged();

            }
            return false;
        }


        public async void Dispose()
        {

            try
            {


                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }


    }

}

