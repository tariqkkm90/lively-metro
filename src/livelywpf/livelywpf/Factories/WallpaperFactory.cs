﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using livelywpf.Core;
using livelywpf.Core.Wallpapers;
using livelywpf.Models;
using livelywpf.Services;

namespace livelywpf.Factories
{
    public class WallpaperFactory : IWallpaperFactory
    {
        private readonly ILivelyPropertyFactory lpFactory;

        public WallpaperFactory(ILivelyPropertyFactory lpFactory)
        {
            this.lpFactory = lpFactory;
        }

        public IWallpaper CreateWallpaper(ILibraryModel obj, ILivelyScreen display, IUserSettingsService userSettings, bool isPreview = false)
        {
            switch (obj.LivelyInfo.Type)
            {
                case WallpaperType.web:
                case WallpaperType.webaudio:
                case WallpaperType.url:
                    switch (userSettings.Settings.WebBrowser)
                    {
                        case LivelyWebBrowser.cef:

                            return new WebProcess(obj.FilePath,
                                obj,
                                display,
                                lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement),
                                userSettings.Settings.WebDebugPort,
                                userSettings.Settings.CefDiskCache,
                                userSettings.Settings.AudioVolumeGlobal);
                        case LivelyWebBrowser.webview2:
                            return new WebEdge(obj.FilePath, obj, display, lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement));
                    }
                    break;
                case WallpaperType.video:
                    //How many videoplayers you need? Yes.
                    switch (userSettings.Settings.VideoPlayer)
                    {
                        case LivelyMediaPlayer.wmf:
                            return new VideoPlayerWpf(obj.FilePath, obj,
                                display, userSettings.Settings.WallpaperScaling);
                        case LivelyMediaPlayer.libvlc:
                            //depreciated
                            throw new DepreciatedException("libvlc depreciated player selected.");
                        case LivelyMediaPlayer.libmpv:
                            //depreciated
                            throw new DepreciatedException("libmpv depreciated player selected.");
                        case LivelyMediaPlayer.libvlcExt:
                            return new VideoPlayerVlcExt(obj.FilePath, obj, display);
                        case LivelyMediaPlayer.libmpvExt:
                            return new VideoPlayerMpvExt(obj.FilePath, 
                                obj, 
                                display,
                                lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement), 
                                userSettings.Settings.WallpaperScaling);
                         case LivelyMediaPlayer.mpv:
                            return new VideoMpvPlayer(obj.FilePath, obj, display, lpFactory.CreateLivelyPropertyFolder(obj, 
                                display, 
                                userSettings.Settings.WallpaperArrangement),
                                userSettings.Settings.WallpaperScaling, 
                                userSettings.Settings.VideoPlayerHwAccel, 
                                isPreview);
                        case LivelyMediaPlayer.vlc:
                            return new VideoVlcPlayer(obj.FilePath, 
                                obj, 
                                display,
                                userSettings.Settings.WallpaperScaling, 
                                userSettings.Settings.VideoPlayerHwAccel);
                    }
                    break;
                case WallpaperType.gif:
                case WallpaperType.picture:
                    switch (userSettings.Settings.GifPlayer)
                    {
                        case LivelyGifPlayer.win10Img:
                            throw new PluginNotFoundException("xaml island gif player not available.");
                        case LivelyGifPlayer.libmpvExt:
                            return new VideoPlayerMpvExt(obj.FilePath, 
                                obj, 
                                display,
                                lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement),
                                userSettings.Settings.WallpaperScaling);
                        case LivelyGifPlayer.mpv:
                            return new VideoMpvPlayer(obj.FilePath,
                                obj,
                                display,
                                lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement),
                                userSettings.Settings.WallpaperScaling, 
                                userSettings.Settings.VideoPlayerHwAccel, 
                                isPreview);
                    }
                    break;
                case WallpaperType.app:
                case WallpaperType.bizhawk:
                case WallpaperType.unity:
                case WallpaperType.unityaudio:
                case WallpaperType.godot:
                    if (Constants.ApplicationType.IsMSIX)
                    {
                        throw new MsixNotAllowedException("Program wallpaper on MSIX package not allowed.");
                    }
                    else
                    {
                        return new ExtPrograms(obj.FilePath, obj, display,
                            userSettings.Settings.WallpaperWaitTime);
                    }
                case WallpaperType.videostream:
                    if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", "mpv", "youtube-dl.exe")))
                    {
                        return new VideoMpvPlayer(obj.FilePath, 
                            obj,
                            display,
                            lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement),
                            userSettings.Settings.WallpaperScaling, userSettings.Settings.VideoPlayerHwAccel,
                            isPreview, userSettings.Settings.StreamQuality);
                    }
                    else
                    {
                        //note: wallpaper type will be videostream, don't forget..
                        return new WebProcess(obj.FilePath,
                            obj,
                            display,
                            lpFactory.CreateLivelyPropertyFolder(obj, display, userSettings.Settings.WallpaperArrangement),
                            userSettings.Settings.WebDebugPort,
                            userSettings.Settings.CefDiskCache,
                            userSettings.Settings.AudioVolumeGlobal);
                    }
            }
            throw new PluginNotFoundException("Wallpaper player not found.");
        }

        #region exceptions

        public class MsixNotAllowedException : Exception
        {
            public MsixNotAllowedException()
            {
            }

            public MsixNotAllowedException(string message)
                : base(message)
            {
            }

            public MsixNotAllowedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class DepreciatedException : Exception
        {
            public DepreciatedException()
            {
            }

            public DepreciatedException(string message)
                : base(message)
            {
            }

            public DepreciatedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class PluginNotFoundException : Exception
        {
            public PluginNotFoundException()
            {
            }

            public PluginNotFoundException(string message)
                : base(message)
            {
            }

            public PluginNotFoundException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        #endregion //exceptions
    }
}
