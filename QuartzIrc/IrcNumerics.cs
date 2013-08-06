﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzIrc
{
    /// <summary>
    /// IRC Numerics
    /// </summary>
    public enum IrcNumerics
    {
        Welcome = 1,
        YourHost = 2,
        Created = 3,
        MyInfo = 4,
        Bounce = 5,
        Userhost = 302,
        IsOn = 303,
        Away = 301,
        Unaway = 305,
        NowAway = 306,
        WhoisUser = 311,
        WhoisServer = 312,
        WhoisOperator = 313,
        WhoisIdle = 317,
        WhoisEnd = 318,
        WhoisChannels = 319,
        WhoWasUser = 314,
        WhoWasEnd = 369,
        ListReply = 322,
        ListEnd = 323,
        NoTopic = 331,
        Topic = 332,
        TopicSetter = 333,
        Inviting = 341,
        InviteList = 346,
        InviteListEnd = 347,
        ExceptList = 348,
        ExceptListEnd = 349,
        Version = 351,
        WhoReply = 352,
        WhoEnd = 315,
        NamesReply = 353,
        NamesEnd = 366,
        LinksReply = 364,
        LinksEnd = 365,
        BanListReply = 367,
        BanListEnd = 368,
        Info = 371,
        InfoEnd = 374,
        MotdStart = 375,
        Motd = 372,
        MotdEnd = 376,
        OperUp = 381,
        Rehash = 382,
        ServiceUp = 383,
        Time = 391,
        TraceLink = 200,
        TraceConnecting = 201,
        TraceHandshake = 202,
        TraceUnknown = 203,
        TraceOperator = 204,
        TraceUser = 205,
        TraceServer = 206,
        TraceService = 207,
        TraceNewType = 208,
        TraceClass = 209,
        TraceReconnect = 210,
        TraceLog = 261,
        TraceEnd = 262,
        StatsLinkInfo = 211,
        StatsCommands = 212,
        StatsEnd = 219,
        StatsUptime = 242,
        StatsOline = 243,
        UModeIs = 221,
        ServerList = 234,
        ServerListEnd = 235,
        LuserClient = 251,
        LuserOp = 252,
        LuserUnknown = 253,
        LuserChannels = 254,
        LuserMe = 255,
        AdminMe = 256,
        AdminLoc1 = 257,
        AdminLoc2 = 258,
        AdminEmail = 259,
        TryAgain = 263,

        NoSuchNick = 401,
        NoSuchServer = 402,
        NoSuchChannel = 403,
        CannotSendToChannel = 404,
        TooManyChannels = 405,
        WasNoSuchNick = 406,
        TooManyTargets = 407,
        NoSuchService = 408,
        NoOrigin = 409,
        NoRecipient = 411,
        NoTextToSend = 412,
        NoTopLevel = 413,
        WildTopLevel = 414,
        BadMask = 415,
        UnknownCommand = 421,
        NoMotd = 422,
        NoAdminInfo = 423,
        FileError = 424,
        NoNicknameGiven = 431,
        ErroneusNick = 432,
        NickInUse = 433,
        NickCollision = 436,
        UnavailableResource = 437,
        UserNotInChannel = 441,
        NotOnChannel = 442,
        UserOnChannel = 443,
        NoLogin = 444,
        NotRegistered = 451,
        NeedMoreParams = 461,
        AlreadyRegistered = 462,
        NoPermissionForHost = 463,
        PasswordMismatch = 464,
        YouAreBannedCreep = 465,
        YouWillBeBanned = 466,
        KeySet = 467,
        ChannelIsFull = 471,
        UnknownMode = 472,
        InviteOnlyChan = 473,
        BannedFromChan = 474,
        BadChannelKey = 475,
        BadChannelMask = 476,
        NoChannelModes = 477,
        BanListFull = 478,
        NoPrivileges = 481,
        ChannelOpPrivilegesNeeded = 482,
        CantKillServer = 483,
        Restricted = 484,
        NoOperHost = 491,
        UModeUnknownFlag = 501,
        UsersDontMatch = 502
    }
}
