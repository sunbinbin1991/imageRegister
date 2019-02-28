using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Uface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UImage
    {
        public int Width;
        public int Height;
        public IntPtr pixels;
    }

    class PInvoke
    {
        //const string dll_path = "F:\\maxiaofang\\repos\\uface_stable_master\\build_x86\\app\\quality_judge\\c\\RelWithDebInfo\\uface_quality_judge_c.dll";
        //const string dll_path = "D:\\binbin\\quality_judge\\dll\\uface_quality_judge_c.dll";
        const string dll_path = "uface_quality_judge_c.dll";
        [DllImport(dll_path, EntryPoint = "new_QualityJudge", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_QualityJudge(int min_face_size);

        [DllImport(dll_path, EntryPoint = "delete_QualityJudge", CallingConvention = CallingConvention.Cdecl)]
        public static extern void delete_QualityJudge(IntPtr cptr);

        [DllImport(dll_path, EntryPoint = "QualityJudge_isQualified", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QualityJudge_isQualified(IntPtr cptr, UImage uimage);
    }
}
