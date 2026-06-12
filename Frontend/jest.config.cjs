module.exports = {
  moduleNameMapper: {
    '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
  },
  preset: 'ts-jest',
  setupFilesAfterEnv: ['<rootDir>/src/test/setupTests.ts'],
  testEnvironment: 'jsdom',
  testMatch: ['<rootDir>/src/**/*.test.{ts,tsx}'],
  transform: {
    '^.+\\.(ts|tsx)$': [
      'ts-jest',
      {
        tsconfig: {
          esModuleInterop: true,
          jsx: 'react-jsx',
          module: 'commonjs',
          moduleResolution: 'node',
          target: 'es2020',
          types: ['jest', 'node'],
        },
      },
    ],
  },
};
