const { execSync } = require('child_process')
const core = require('@actions/core')

async function run() {
  try {
    execSync('git fetch --tags')
    const tags = execSync(`git tag -l`).toString()

    const format = core.getInput('format')
    const regex = new RegExp(format, 'g')
    const matchedTags = [...tags.matchAll(regex)]

    let nextBuildNumber = 1
    matchedTags.forEach(match => {
      const buildNumber = Number(match[1])
      if (buildNumber >= nextBuildNumber) {
        nextBuildNumber = buildNumber + 1
      }
    })

    core.setOutput('next', nextBuildNumber)
  } catch (error) {
    core.setFailed(error.message)
  }
}

run()