declare global {
  interface Window {
    $localize?: LocalizeFn;
  }

  var $localize: LocalizeFn | undefined;
}

type LocalizeFn = (messageParts: TemplateStringsArray, ...expressions: unknown[]) => string;

if (globalThis.$localize === undefined) {
  globalThis.$localize = (messageParts: TemplateStringsArray, ...expressions: unknown[]): string => {
    let result = messageParts[0] ?? '';

    expressions.forEach((expression, index) => {
      result += `${expression}${messageParts[index + 1] ?? ''}`;
    });

    return result;
  };
}

export {};