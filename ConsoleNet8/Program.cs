// See https://aka.ms/new-console-template for more information
using Pluralize.NET.Core;
using PluralizeService.Core;

string word = "Person";

string pluralWord = PluralizationProvider.Pluralize(word);
Console.WriteLine(pluralWord);


////string word = "Box";
////string plural = PluralizationProvider.Pluralize(word);
////string singularizedural = PluralizationProvider.Singularize(word);

////Console.WriteLine(plural);
////Console.WriteLine(singularizedural);




//var singular = new Pluralizer().Singularize("Cacti");

//Console.WriteLine(singular);
