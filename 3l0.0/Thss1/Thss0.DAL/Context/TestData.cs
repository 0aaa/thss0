namespace Thss0.DAL.Context
{
    internal class TestData
    {
        public static void Initialize(Thss0Context cntxt)
        {
            string[] testValuesArr = new string[] { "test" };
            if (!cntxt.Procedures.Any())
            {
                for (ushort i = 0; i < testValuesArr.Length; i++)
                {
                    cntxt.Add(new Procedure
                    {
                        Name = testValuesArr[i],
                        Department = testValuesArr[i],
                        CreationTime = DateTime.Now,
                        RealizationTime = DateTime.Now,
                        NextProcedureTime = DateTime.Now,
                        Result = testValuesArr[i]
                    });
                }
                cntxt.SaveChanges();
            }
        }
    }
}