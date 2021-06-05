using System;

namespace Tests.Diagnostics
{
    public class Program
    {
        public void DefaultInitalization()
        {
            Guid default_value = default; // Noncompliant
            var default_T_value = default(Guid); // Noncompliant
        }

        public void WithoutArguments()
        {
            var result = new Guid(); // Noncompliant {{Use 'Guid.NewGuid()' or 'Guid.Empty' or add arguments to this Guid instantiation.}}
            //           ^^^^^^^^^^
        }

        public void WithArguments()
        {
            var bytes = new Guid(new byte[0]); // Compliant
        }

        public void Other()
        { 
            var empty = Guid.Empty; // Compliant
            var rnd = Guid.NewGuid(); // Compliant
        }
    }
}
