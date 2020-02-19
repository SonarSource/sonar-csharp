﻿/*
 * SonarQube, open source software quality management tool.
 * Copyright (C) 2008-2013 SonarSource
 * mailto:contact AT sonarsource DOT com
 *
 * SonarQube is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * SonarQube is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Xml;

namespace Test
{
    public static class XmlTextReaderTest
    {
        private const string Url = "resources/";

        public static XmlTextReader XmlTextReader_OnlyConstructor()
        {
            return new XmlTextReader(Url); // this is safe in .NET 4.5.2+ by default
        }

        public static XmlTextReader XmlTextReader_SetUnsafeResolver(XmlUrlResolver parameter)
        {
            var reader = new XmlTextReader(Url); // this is safe in .NET 4.5.2+ by default
            reader.XmlResolver = parameter; // Noncompliant (S2755) {{Disable access to external entities in XML parsing.}}
            return reader;
        }

        public static void XmlTextReader_SetProhibit()
        {
            var reader = new XmlTextReader(Url);
            reader.DtdProcessing = DtdProcessing.Prohibit; // ok
        }

        public static void XmlTextReader_SetIgnoreProcessing()
        {
            var reader = new XmlTextReader(Url);
            reader.DtdProcessing = DtdProcessing.Ignore; // ok
        }

    }
}
