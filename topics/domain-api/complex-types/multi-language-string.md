# Multilanguage string (Complex value)

Some text properties support value in more than one language.
This properties are of multi-language string type.

The multi-language string can store many language strings, indexing them by language key.
The language key is a CultureInfo two letter ISO language name, like "en", "de", etc.

For example, the name of a product is a data attribute, which can simultaneously contain translation of the product name in many languages.

When you retrieve the value of this attribute with the Domain API, you get values similar to:

```json
{
  "en": "Apple",
  "de": "Apfel"
}
```

### All translations are a single value

All translations in a multi-language string are treated as a single value.
You cannot change only one language pair - all pairs are updated simultaneously.
The client applications are responsible for managing all language pairs.

### Filtering

The equality comparison for multi-language string is ambiguous.
The APIs generally do not support direct equality comparisons.

> [!note]
> Domain API supports only the filter function **contains**.

For example, in Domain API, the following is supported:

```odata
~/General_Products_Products?$filter=contains(Name,'ppl')  
```

However, this is not valid:

```odata
~/General_Products_Products?$filter=Name eq 'Apple' 
```
