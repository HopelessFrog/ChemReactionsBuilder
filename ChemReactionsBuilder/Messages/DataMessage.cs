using ChemReactionsBuilder.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ChemReactionsBuilder.Messages;

public class DataMessage : ValueChangedMessage<Export>
{
    public DataMessage(Export value) : base(value)
    {
    }
}