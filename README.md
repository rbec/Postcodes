# UK Postcode Data Structure

## Overview
* [Type safe](https://en.wikipedia.org/wiki/Type_safety) representation of only those strings that are valid postcodes
* Stack allocated 4 byte struct instead of 26 - 30 bytes for a heap allocated string + 4 bytes for a reference to it
* Fast postcode validation and parsing, removing spaces anywhere in the input string and treating all letters case in-sensitively

Every UK address is associated with a [postcode](https://en.wikipedia.org/wiki/Postcodes_in_the_United_Kingdom). This consists of between 5 and 7 letters and digits in one of these formats:

| Area | Sector | Unit |
|------|--------|------|
| A    | 1      | 1AA  |
| A    | 11     | 1AA  |
| A    | 1A     | 1AA  |
| AA   | 1      | 1AA  |
| AA   | 11     | 1AA  |
| AA   | 1A     | 1AA  |

Where `A` represents an upper case letter A-Z and `1` represents a digit 0-9. A single space is placed between the sector and unit for a total of between 6 and 8 characters.

### Description
* 1st character of the Area is a letter (26 possibilities)
* 2nd character of the Area is a letter or missing (26 + 1 = 27 possibilities)
* 1st character of the Sector is a digit (10 possibilities)
* 2nd character of the Sector is a letter, a digit or missing (26 + 10 + 1 = 37 possibilities)
* 1st character of the Unit is a digit (10 possibilities)
* 2nd character of the Unit is a letter (26 possibilities)
* 3rd character of the Unit is a letter (26 possibilities)

Hence the number of possible postcodes is
26 · 27 · 10 · 37 · 10 · 26 · 26 = 1,755,842,400 ≤ 2³²

Therefore it is possible to represent a postcode in a 4 byte (32 bit) word by using this scheme

| Value                    | 0 | 1 | 2 | 3 | … | 9 | 10 | 11 | 12 | 13 | …  | 25 | 26 | … | 36 | 
|--------------------------|---|---|---|---|---|---|--- |----|----|----|----|----|----|---|----|
| Digit                    | 0 | 1 | 2 | 3 | … | 9 |
| Letter                   | A | B | C | D | … | J | K  | L  | M  | N  | …  | Z  |
| Letter or missing        |   | A | B | C | … | K | L  | M  | N  | O  | …  | Y  |  Z |
| Letter, digit or missing |   | 0 | 1 | 2 | … | 8 | 9  | A  | B  | C  | …  | M  |  N | … |  Z |

#### Parsing Algorithm
``` C#
public static bool TryParse(string s, out Postcode postcode)
```
Imagine each valid postcode is placed in a [7 dimensional array](https://en.wikipedia.org/wiki/Array_data_structure#Multidimensional_arrays) with the indexes for each dimension given by the above scheme above. This array might be represented in memory by a single dimensional array. By calculating the index for a given postcode in this single dimensional array we have an integer that uniquely specifies any valid postcode. It is not necessary to store the hypothetical array since the element the index represents can be easily computed using the reverse calculation.
