﻿// Copyright (c) https://github.com/sidiandi 2016
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public enum PlaybackModes
    {
        StopAfterEverything,
        StopAfterEveryAlbum,
        StopAfterEveryTrack,
        LoopEverything,
        LoopAlbum,
        LoopTrack
    }

    public class Package
    {
        public Package()
        {
            Tracks = new Track[] { };
            NextOid = 10250;
        }

        public Package(IProductIdProvider productIdProvider)
            :this()
        {
            StopOid = GetNextOid();
            ProductId = productIdProvider.GetNextAvailableProductId();
        }

        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(fileName))
                {
                    return String.Format("ProductId{0}", ProductId);
                }
                return fileName;
            }

            set
            {
                fileName = value; }
        }

        string fileName;

        public string Title { get; set; }

        public int ProductId { get; set; }

        public PlaybackModes PlaybackMode;

        public int NextOid {get; set; }

        bool sortOid = false;

        public bool SortOid {
            get
            {
                return sortOid;
            }
            set
            {
                sortOid = value;
                if (sortOid)
                {
                    ReOid();
                }
            }
        }

        public int GetNextOid()
        {
            return NextOid++;
        }

        public Track[] Tracks
        {
            get; set;
        }

        public int StopOid { get; set; }

        public string ConfirmationSound { set; get; }
        public IEnumerable<Album> Albums
        {
            get
            {
                return Tracks.GroupBy(_ => _.Album)
                    .Select(tracks => new Album
                    {
                        Title = tracks.Key,
                        Tracks = tracks.ToArray()
                    });
            }
        }

        public static Package CreateFromInputPaths(IEnumerable<string> inputPaths)
        {
            var albumReader = new AlbumReader();

            var audioFiles = albumReader.GetAudioFiles(inputPaths);

            var package = new Package(new ProductIdProvider())
            {
                Tracks = albumReader.GetTracks(audioFiles)
            };

            var artists = package.Tracks.SelectMany(track => track.Artists).Distinct();
            package.Title = String.Join(", ", artists);

            return package;
        }

        /// <summary>
        /// Add all new tracks found in inputfiles
        /// </summary>
        /// <param name="inputFiles"></param>
        public void AddTracks(IEnumerable<string> inputFiles)
        {
            var albumReader = new AlbumReader();
            var existing = Tracks.ToLookup(_ => _.Path);
            var toAdd = albumReader.GetTracks(albumReader.GetAudioFiles(inputFiles))
                .Where(t => !existing.Contains(t.Path));

            foreach (var track in toAdd)
            {
                track.Oid = this.GetNextOid();
            }

            Tracks = Tracks.Concat(toAdd)
                .OrderBy(_ => _.Album)
                .ThenBy(_ => _.TrackNumber)
                .ToArray();

            if (SortOid)
            {
                ReOid();
            }
        }

        public void ReOid()
        {
            NextOid = StopOid + 1;
            foreach(var track in Tracks)
            {
                track.Oid = this.GetNextOid();
            }
        }

        public void RemoveTracks(IEnumerable<Track> enumerable)
        {
            Tracks = Tracks.Except(enumerable).ToArray();
        }
    }
}
