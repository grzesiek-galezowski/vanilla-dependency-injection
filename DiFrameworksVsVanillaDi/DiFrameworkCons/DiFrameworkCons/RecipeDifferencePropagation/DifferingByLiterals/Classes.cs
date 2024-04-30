namespace DiFrameworkCons.MultipleRecipesForTheSameTypesAndNamingPropagation.DifferingByLiterals;

public record World(Character Hero, Character Enemy);
public record Character(Armor Armor, Sword Sword);
public record Armor(Helmet Helmet, BreastPlate BreastPlate);
public record BreastPlate(int Defense);
public record Helmet;
public record Sword(int Attack);