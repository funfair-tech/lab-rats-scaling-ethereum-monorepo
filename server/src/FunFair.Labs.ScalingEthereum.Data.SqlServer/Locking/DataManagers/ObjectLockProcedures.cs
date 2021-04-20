namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.DataManagers
{
    /// <summary>
    ///     The Stored procedures that should be used to access the database lock tables
    /// </summary>
    public sealed record ObjectLockProcedures
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="clear">Stored procedure to be called that clears any locks taken by this computer</param>
        /// <param name="acquire">Stored procedure to be called that attempts to acquire a lock.</param>
        /// <param name="release">Stored procedure to be called that releases a specific lock.</param>
        /// <param name="isLocked">Stored procedure to be called that gets the status of a lock for an object.</param>
        public ObjectLockProcedures(string clear, string acquire, string release, string isLocked)
        {
            this.Clear = clear;
            this.Acquire = acquire;
            this.Release = release;
            this.IsLocked = isLocked;
        }

        /// <summary>
        ///     Stored procedure to be called that clears any locks taken by this computer
        /// </summary>
        /// <remarks>
        ///     Parameters:
        ///     * MachineName varchar(100)
        /// </remarks>
        public string Clear { get; }

        /// <summary>
        ///     Stored procedure to be called that attempts to acquire a lock.
        /// </summary>
        /// <remarks>
        ///     Parameters:
        ///     * ObjectId (as appropriate)
        ///     * MachineName varchar(100)
        ///     Returns table:
        ///     * ObjectId (as appropriate)
        ///     * LockedBy varchar(100)
        ///     * LockedAt DateTime2
        /// </remarks>
        public string Acquire { get; }

        /// <summary>
        ///     Stored procedure to be called that releases a specific lock.
        /// </summary>
        /// <remarks>
        ///     Parameters:
        ///     * ObjectId (as appropriate)
        /// </remarks>
        public string Release { get; }

        /// <summary>
        ///     Stored procedure to be called that gets the status of a lock for an object.
        /// </summary>
        /// <remarks>
        ///     Parameters:
        ///     * ObjectId (as appropriate)
        ///     Returns table:
        ///     * ObjectId (as appropriate)
        ///     * LockedBy varchar(100)
        ///     * LockedAt DateTime2
        /// </remarks>
        public string IsLocked { get; }
    }
}