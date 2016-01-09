// Copyright (c) https://github.com/sidiandi 2016
// 
// This file is part of tta.
// 
// tta is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// tta is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttab
{
    public class TrackInfo
    {
        public int Oid;
        public string Id;
        
        /// <summary>
        /// Length of the media file
        /// </summary>
        public long Length;
    }

    public class AlbumCollectionMeta
    {
        [XmlIgnore]
        public Dictionary<string, TrackInfo> TrackInfo = new Dictionary<string, TrackInfo>();

        public AlbumCollection AlbumCollection;

        public int StartOid = 10250;
        public string Name;
        public int ProductId;

        public string YamlFile;
        public string HtmlFile;
        public string GmeFile;

        public int StopOid;
    }

}
