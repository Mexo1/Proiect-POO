namespace Proiect_POO.ValueObjects;
    public readonly struct DonationId(Guid  Value)
    {
        public static DonationId New()=>new DonationId(Guid.NewGuid());
    }
