using System;

namespace Obfuscar.MsBuild.Tests {
    public class PublicClass {
        public void DoTest() {
            DoTestInternal();
        }

        private void DoTestInternal() {
            var V1 = new InternalClass();
            V1.DoTestInternal();
        }

    }

}
