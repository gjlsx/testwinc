using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.test
{
    class TestACMPoint24
    {
        //double PRECISION = 1E-6;
      //  static int ico = 4;
        static string sExpre = "";
       
        class RType 
        {
            public static int iNum = 4;
            public static int iValue = 24;
            public double[] ire;  //中间值
            public String[] sexp; //表达式
        }

        String[] smath = { "+", "-", "*", "/" };

        //本程序 求24点。输入4个数据的范围1-13
        //a,b,c,d如果可以组成24点，can24返回1，不能则can24返回0
        //（此外，你可以在can24内调用你写的另外一个函数）
        //by wind,  2013.11.10
        public static int can24(int a, int b, int c, int d)
        {
            if (a < 1 || b < 1 || c < 1 || d < 1 || a > 13 || b > 13 || c > 13 || d > 13)
            {
                sExpre = "";
                return 0;
            }


            if (run24(4, a, b, c, d, null))
                return 1;
            else
            {
                sExpre = "";
                return 0;
            }


        }

        public static string getExpress()
        {
            return sExpre;
        }

        private static Boolean run24(int n, double a, double b, double c,double d,String sexp)
        {
            if (n == 2)
            {
                RType rt = runab(a, b,"","");
                double[] iab =rt.ire;
                String[] sans = rt.sexp;
                for (int ik = 0; ik < iab.Length; ik++)
                {
                    if (is24OK(iab[ik], 24))
                    {
                        sExpre = sans[ik];
                        return true;
                    }
                }

            }
            if (n == 3)
            {
                RType iab = runab(a, b, sexp,"");
                RType iac = runab(a, c, sexp,"");
                RType ibc = runab(b, c, "","");
                return isrunab(c, iab,"") || isrunab(b, iac,"") || isrunab(a, ibc,sexp);
            }
            if (n == 4)
            {
                RType iab = runab(a, b,"","");
                RType iac = runab(a, c,"","");
                RType ibc = runab(b, c,"","");
                RType iad = runab(a, d,"","");
                RType ibd = runab(b, d,"","");
                RType icd = runab(c, d,"","");
                return runab3(iab,c,d)||runab3(iac,b,d)||runab3(ibc,a,d) ||runab3(iad,b,c)||runab3(ibd,a,c) ||runab3(icd,a,b);
            }

            return false;
        }

        private static Boolean runab3(RType dl, double b, double d)
        {
            Boolean isanswer = false;
            double[] dbtmp = dl.ire;
            String[] stmp = dl.sexp;
            for (int it = 0; it < dbtmp.Length; it++)
            {
                double a = dbtmp[it];
                if (run24(3, a, b, d, 0, stmp[it]))
                {
                    isanswer = true;
                    return isanswer;
                }
            }
            return isanswer;

        }

        private static RType runab(double a, double b, String sexp,String sexb)
        {
            double[] ire = new double[6];
            String[] sex = new String[6];
            ire[0] = a + b;
            ire[1] = a - b;
            ire[2] = a * b;
            ire[3] = a / b;
            ire[4] = b - a;
            ire[5] = b / a;
            String sa = a+"";
            String sb = b + "";
            if (!(sexb.Equals("")))
            {
                sb = sexb;
            }
            if (!(sexp.Equals("")))
            {
                sa = sexp;
            }
                sex[0] = " (" + sa + " + " + sb + ") ";
                sex[1] = " (" + sa + " - " + sb + ") ";
                sex[2] = " (" + sa + " * " + sb + ") ";
                sex[3] = " (" + sa + " / " + sb + ") ";
                sex[4] = " (" + sb + " - " + sa + ") ";
                sex[5] = " (" + sb + " / " + sa + ") ";

            RType rt = new RType();
            rt.ire = ire;
            rt.sexp = sex;
            return rt;
        }

        private static Boolean isrunab(double a, RType c,String stra)
        {
            int resu = 24;
            double[] dbtmp = c.ire;
            String[] stmp = c.sexp;
            for (int it = 0; it < dbtmp.Length; it++)
            {

                double b = dbtmp[it];
                RType rt = runab(b, a, stmp[it], stra);
                double[] abrun = rt.ire;

               
                for (int ik = 0; ik < abrun.Length; ik++)
                {
                    if (is24OK(abrun[ik], resu))
                    {
                        sExpre = rt.sexp[ik];
                        return true;
                    }
                }
             }
            return false;
        }

        private static Boolean is24OK(double dvalue, int RESULT)
        {
            double PRECISION = 1E-6;
            double dtmp = dvalue - RESULT;
            if (dtmp < 0)
            { dtmp = 0 - dtmp; }
            if (dtmp < PRECISION)
            {
                return true;
            }
            else
                return false;
        }


    }
}
