# Next Build Number

Determine the next build number to use based on a given format. If we have both dev and beta pre-releases, we want the version build numbers to increment independently from one another rather than be based on a global run number. This is accomplished with a regex format parameter.

## Inputs

### `format`

**Required**
Provide a regular expression to match other git tags with the same incrementing build number. Capture the build number.

Example: `1.0.0-beta(\d+)`

See this action's own [distribute.yml](./.github/workflows/distribute.yml) action to see how we set the format based on the `package.json` version and current pre-release branch.

## Outputs

### `next`

The next build number.