﻿namespace livelywpf.Cmd
{
    public interface ICommandHandler
    {
        /// <summary>
        /// Parse commandline args.
        /// </summary>
        /// <param name="args"></param>
        void ParseArgs(string[] args);
    }
}