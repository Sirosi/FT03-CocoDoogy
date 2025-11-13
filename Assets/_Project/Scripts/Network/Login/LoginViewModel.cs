using CocoDoogy.Data;
using CocoDoogy.Network.UI;
using CocoDoogy.UI.Popup;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace CocoDoogy.Network.Login
{
    public class LoginViewModel
    {
        private readonly AuthProvider authProvider;

        public FirebaseUser CurrentUser { get; private set; }
        public string ErrorMessage { get; private set; }

        public event Action<FirebaseUser> OnUserChanged;
        public event Action<string> OnErrorChanged;
        public event Action OnLoggedOut;

        public LoginViewModel(AuthProvider provider)
        {
            authProvider = provider;
            authProvider.OnLoginSuccess += HandleLoginSuccess;
            authProvider.OnLoginFailed += HandleLoginFailed;
            authProvider.OnLogout += HandleLogout;
        }

        #region < 로그인 관련 기능 >
        private async void HandleLoginSuccess(FirebaseUser user)
        {
            bool isExistingUser = await UserData.CheckUserProfileExistsAsync(user.UserId);

            if (!isExistingUser) // 기존 유저가 아닌 경우
            {
                string nickname = await GetValidNickNameAsync(user.UserId);

                await UserData.CreateOnServerAsync(user.UserId, nickname);
            }

            CurrentUser = user;
            DataManager.Instance.StartListeningForUserData(CurrentUser.UserId);
            _ = FirebaseManager.Instance.RechargeTicketAsync();
            OnUserChanged?.Invoke(user);
        }
        private void HandleLoginFailed(string error)
        {
            ErrorMessage = error;
            OnErrorChanged?.Invoke(error);
        }
        private void HandleLogout()
        {
            CurrentUser = null;
            OnLoggedOut?.Invoke();
        }
        public void SignIn() => authProvider.SignInWithGoogle();
        public void SignOut() => authProvider.SignOut();
        #endregion
        
        #region < 로그인 이후 최초 가입 시 닉네임 입력 시퀀스 >

        private async Task<string> GetValidNickNameAsync(string uid)
        {
            string nickname = null;
            bool isSuccess = false;

            while (!isSuccess)
            {
                nickname = await ShowNicknameInputDialogAsync();

                if (string.IsNullOrWhiteSpace(nickname))
                {
                    await ShowMessageAsync("알림", "닉네임은 비워둘 수 없습니다!");
                    continue; 
                }

                try
                {
                    bool isAvailable = await UserData.TrySetNewNickNameAsync(uid, nickname);

                    if (isAvailable)
                    {
                        isSuccess = true; 
                    }
                    else
                    {
                        await ShowMessageAsync("알림", $"'{nickname}'은 이미 사용 중인 닉네임입니다. 다른 닉네임을 사용해주세요.");
                    }
                }
                catch (Exception ex)
                {
                    await ShowMessageAsync("오류", $"닉네임 검사 중 오류가 발생했습니다: {ex.Message}");
                }
            }

            return nickname;
        }

        private Task ShowMessageAsync(string title, string message)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            MessageDialog.ShowMessage(title, message, DialogMode.Confirm, (_) => taskCompletionSource.SetResult(true));

            return taskCompletionSource.Task;
        }

        private Task<string> ShowNicknameInputDialogAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            MessageDialog.ShowInputBox(
                "닉네임 설정",
                "처음 오신 것 같아요!\n사용하실 닉네임을 입력해주세요.",
                (nickname) => taskCompletionSource.SetResult(nickname));

            return taskCompletionSource.Task;
        }

        #endregion

        #region < 익명로그인 기능 & 익명로그인 링크 구글 기능 > 
        public void SignInAnonymously() => authProvider.SignInAnonymously();
        public void LinkGoogleAccount() =>  authProvider.LinkGoogleAccount();
        #endregion
    }
}