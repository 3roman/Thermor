using System.Runtime.InteropServices;

namespace ThermoMate.Utility
{
    static class UEwasp
    {
        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void SETSTD_WASP(int stdid);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2T(double p, ref double t, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2VL(double p, ref double v, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2VG(double p, ref double v, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2HL(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2HG(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2KSG(double p, ref double ks, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2KSL(double p, ref double ks, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2ETAL(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2ETAG(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2P(double t, ref double p, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2VL(double p, ref double v, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2VG(double p, ref double v, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2HL(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2HG(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2KSG(double p, ref double ks, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2KSL(double p, ref double ks, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2ETAL(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2ETAG(double p, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2V(double p, double t, ref double v, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2H(double p, double t, ref double h, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2ETA(double p, double t, ref double cp, ref int r);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2KS(double p, double t, ref double ks, ref int r);
    }
}
