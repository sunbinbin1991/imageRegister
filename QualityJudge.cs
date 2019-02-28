using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uface
{
    public class QualityJudge
    {
        private IntPtr cptr;
        public QualityJudge(int min_face_size)
        {
            cptr = PInvoke.new_QualityJudge(min_face_size);

            Console.WriteLine("p 1");
        }

        ~QualityJudge()
        {
            PInvoke.delete_QualityJudge(cptr);

            Console.WriteLine("p 2");
        }


        public int isQualified(UImage uimage)
        {
            int res = -100;

            res = PInvoke.QualityJudge_isQualified(cptr, uimage);

            return res;
        }
    }
}
