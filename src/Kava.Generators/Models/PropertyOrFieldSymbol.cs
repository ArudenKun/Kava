using Dunet;

namespace Kava.Generators.Models;

[Union]
public partial record PropertyOrFieldSymbol
{
    partial record Property(IPropertySymbol PropertySymbol);

    partial record Field(IFieldSymbol FieldSymbol);
}
