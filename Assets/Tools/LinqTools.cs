using System;
using System.Collections.Generic;

// Custom Linq extensions.
public static class MyExtensions {

    // Linq to get the min of a value of a list of structs/classes.
    // Usage int min = List<Struct>.MinOf((t) => t.int);
    // Ex. int min = population.MinOf((t) => t.fitness);
    public static TSource MinOf<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, double> selector) {
        // Get the enumerator.
        var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("The source sequence is empty.");

        // Take the first value as a minimum.
        double minimum = selector(enumerator.Current);
        TSource current = enumerator.Current;

        // Go through all other values...
        while (enumerator.MoveNext()) {
            double value = selector(enumerator.Current);
            if (value < minimum) {
                // A new minimum was found. Store it.
                minimum = value;
                current = enumerator.Current;
            }
        }

        // Return the minimum value.
        return current;
    }

    // Linq to get the max of a value of a list of structs/classes.
    // Usage int max = List<Struct>.MaxOf((t) => t.int);
    // Ex. int max = population.MaxOf((t) => t.fitness);
    public static TSource MaxOf<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, double> selector) {
        // Get the enumerator.
        var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("The source sequence is empty.");

        // Take the first value as a maximum.
        double maximum = selector(enumerator.Current);
        TSource current = enumerator.Current;

        // Go through all other values...
        while (enumerator.MoveNext()) {
            double value = selector(enumerator.Current);
            if (value > maximum) {
                // A new maximum was found. Store it.
                maximum = value;
                current = enumerator.Current;
            }
        }

        // Return the maximum value.
        return current;
    }
}