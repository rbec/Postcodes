# UK Postcode Data Structure

## Description

Every UK address is associated with a [postcode](https://en.wikipedia.org/wiki/Postcodes_in_the_United_Kingdom). This consists of between 5 and 7 letters and digits in one of these formats:

| Area | Sector | Unit |
|------|--------|------|
| A    | 1      | 1AA  |
| A    | 11     | 1AA  |
| A    | 1A     | 1AA  |
| AA   | 1      | 1AA  |
| AA   | 11     | 1AA  |
| AA   | 1A     | 1AA  |

Where `A` represents an upper case letter A-Z and `1` represents a digit 0-9. A single space is placed between the sector and unit. This type represents a postcode according to this definition.

### Benefits
* [Type safe](https://en.wikipedia.org/wiki/Type_safety) representation of only those strings that are valid postcodes
* Stack allocated 4 byte struct instead of 26 - 30 bytes for a heap allocated string + 4 bytes for a pointer to it
* 
### Observations
* 1st character of the Area is a letter (26 possibilities)
* 2nd character of the Area is a letter or missing (26 + 1 = 27 possibilities)
* 1st character of the Sector is a digit (10 possibilities)
* 2nd character of the Sector is a letter, a digit or missing (26 + 10 + 1 = 37 possibilities)
* 1st character of the Unit is a digit (10 possibilities)
* 2nd character of the Unit is a letter (26 possibilities)
* 3rd character of the Unit is a letter (26 possibilities)

Hence the number of possible postcodes is
26 · 27 · 10 · 37 · 10 · 26 · 26 = 1,755,842,400 ≤ 2³²

## Motivation
