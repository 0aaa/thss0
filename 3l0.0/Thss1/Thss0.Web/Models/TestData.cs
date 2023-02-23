namespace Thss0.DAL.Context
{
    internal class TestData
    {
        public static void Initialize()
        {
            string[] testValuesArr = new string[] { "test" };
            /*if (!_rleMngr.Roles.Any())
            {
                for (ushort i = 0; i < testValuesArr.Length; i++)
                {
                    _rleMngr.CreateAsync(new IdentityRole { Name = testValuesArr[i] }).Wait();
                }
            }
            if (!_usrMngr.Users.Any())
            {
                for (ushort i = 0; i < testValuesArr.Length; i++)
                {
                    _usrMngr.CreateAsync(new IdentityUser
                    {
                        UserName = testValuesArr[i],
                        Email = testValuesArr[i],
                        PhoneNumber = testValuesArr[i]
                    }, testValuesArr[i]).ContinueWith(delegate
                    {
                        _usrMngr.AddToRoleAsync(_usrMngr.FindByNameAsync(testValuesArr[i]).Result, testValuesArr[i]).Wait();
                    }).Wait();
                }
            }*/
        }
    }
}
