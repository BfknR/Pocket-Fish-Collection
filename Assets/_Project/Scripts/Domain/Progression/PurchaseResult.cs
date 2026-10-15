namespace PocketFish.Domain.Progression
{
    /// <summary>The outcome of trying to buy an upgrade or an aquarium.</summary>
    public enum PurchaseResult
    {
        /// <summary>Bought. Coins were spent.</summary>
        Success,
        /// <summary>The player can't afford it. Nothing changed.</summary>
        NotEnoughCoins,
        /// <summary>The item is already at its top level.</summary>
        AlreadyMaxLevel,
        /// <summary>The aquarium isn't owned yet.</summary>
        NotOwned,
        /// <summary>Nothing to buy: the current aquarium isn't fully upgraded or there are no more aquariums.</summary>
        NotAvailable,
        /// <summary>The aquarium or item doesn't exist.</summary>
        Unknown
    }
}
