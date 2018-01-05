﻿using System;

namespace Tests.Diagnostics
{
    class Foo
    {
        private void Test()
        {
            byte b = 1 << 10; // Noncompliant {{Either promote shift target to a larger integer type or shift by 2 instead.}}
//                   ^^^^^^^
            b = 1 << 10; // Noncompliant {{Either promote shift target to a larger integer type or shift by 2 instead.}}
//              ^^^^^^^
            b = 1 << 0;

            sbyte sb = 1 << 10; // Noncompliant

            int i = 1 << 10;
            i = i << 32;  // Noncompliant
            uint ui = 1 << 32; // Noncompliant
            Int32 i32 = 1 << 32; // Noncompliant
            Int64 i64 = 1 << 32;
            Int64 i64 = 1 << 64; // Noncompliant

            long l = 1 << 32;
            l = 1 << 64; // Noncompliant {{Remove this useless shift by 64.}}
            ulong ul = 1 << 64; // Noncompliant
            ul = 1 << 65; // Noncompliant {{Correct this shift; shift by 1 instead.}}

            ul <<= 0;
            ul <<= 1025; // Noncompliant {{Correct this shift; shift by 1 instead.}}

            b <<= 16; // Noncompliant {{Either promote shift target to a larger integer type or shift by less than 8 instead.}}
            b <<= 17; // Noncompliant {{Either promote shift target to a larger integer type or shift by 1 instead.}}

            int value =
                      (b & 0xff) << 56 // Noncompliant {{Either promote shift target to a larger integer type or shift by 24 instead.}}
                    | (b & 0xff) << 48 // Noncompliant {{Either promote shift target to a larger integer type or shift by 16 instead.}}
                    | (b & 0xff) << 40 // Noncompliant {{Either promote shift target to a larger integer type or shift by 8 instead.}}
                    | (b & 0xff) << 32 // Noncompliant {{Either promote shift target to a larger integer type or shift by less than 32 instead.}}
                    | (b & 0xff) << 24
                    | (b & 0xff) << 16
                    | (b & 0xff) << 8
                    | (b & 0xff) << 0;
        }

        private void NonIntegerTypes()
        {
            object o = 1 << 1024; // Compliant
            IDoNotExist e = 1 << 1024; // Compliant

            double d = 1 << 1024; // Compliant
            float f = 1 << 1024; // Compliant
            decimal m = 1 << 1024; // Compliant
            Single s = 1 << 1024; // Compliant
        }


        private int Property
        {
            get { return 1 << 0; } // Noncompliant
            set { int i = 1 << 0; } // Noncompliant
        }

        private void Lambda()
        {
            var x = () => 1 << 0; // Noncompliant
        }

        private void ParanthesesAttack()
        {
            int i;
            i = (1) << 0; // Noncompliant


            i = (((1))) << 0; // Noncompliant


            i = 1 << (0); // Noncompliant


            i = 1 << (((0))); // Noncompliant


            (((i))) = (((1))) << (((0))); // Noncompliant


            (((i))) <<= (((0))); // Noncompliant
        }

        public byte Next() => 1;

        public int GetInt(ByteOrder byteOrder = ByteOrder.BigEndian)
        {
            return (short)(((Next() & 0xff) << 24)
                        | ((Next() & 0xff) << 16)
                        | ((Next() & 0xff) << 8)
                        | ((Next() & 0xff) << 0));
        }

        private int RightShift()
        {
            int i = i >> 60; // Noncompliant {{Correct this shift; '60' is larger than the type size.}}
            i >>= 60; // Noncompliant {{Correct this shift; '60' is larger than the type size.}}
            int i = i >> 31; // Compliant
            ulong ul = ul >> 64; // Noncompliant {{Correct this shift; '64' is larger than the type size.}}
            i = i >> 0; // Compliant


            i = i >> 0; // Noncompliant {{Remove this useless shift by 0.}}


            ul = ul >> 32; // Compliant
            ul = ul << 0;  // Compliant
        }
    }
}
