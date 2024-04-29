namespace DiFrameworkCons.MultipleRecipesForTheSameTypesAndNamingPropagation.DifferingBySubtype;

public interface IBodyArmor;
public record World(Character Hero, Character Enemy);
public record Character(Armor Armor, IHandWeapon Weapon);
public record Armor(Helmet Helmet, IBodyArmor BodyArmor);
public record BreastPlate : IBodyArmor;
public record ChainMail : IBodyArmor;
public record Helmet;
public interface IHandWeapon;
public record ShortSword : IHandWeapon;
public record LongSword : IHandWeapon;