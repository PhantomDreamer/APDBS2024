// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
Console.WriteLine("Mod1");
Console.WriteLine("Mod2");
Console.WriteLine("Mod3");

int[] arr = { 1, 6, 7, 8, 2, 3, 5, 7, 2, 8 };

static void avgNumbers(int[] calc) {
    double sum = 0;
    for (int i = 0; i < calc.Length; i++) {
        sum += calc[i];
    }
    Console.WriteLine(sum / calc.Length);
}

avgNumbers(arr);